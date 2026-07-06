# Form-URL-Encoded → Object-Tree Binding: AOT & Performance Investigation

> Status: **Investigation + recommendation** (no code yet)
> Date: 2026-06-23
> Scope: deserialization only (form key/value pairs → object graph); read-only.

---

## 1. Problem statement

`Arbor.ModelBinding` binds `application/x-www-form-urlencoded` data (a flat list of
`KeyValuePair<string, StringValues>`) onto a strongly-typed object tree. The current
implementation in `src/Arbor.ModelBinding/FormsParser.cs` works but has structural
liabilities that conflict with the agreed goals — **high performance** and **AOT
compatibility**, simultaneously.

### 1.1 How the current binder works (baseline)

`FormsParser.ParseFromPairs`:

1. Walks the pairs once, splitting them into *flat*, *dotted* (`prop.sub`) and
   *indexed* (`prop[0].sub`) buckets.
2. Recursively builds an **`ExpandoObject`** mirror of the target type's shape (using
   `DeclaredProperties` reflection + `Activator.CreateInstance(List<>)`).
3. **Serializes that `ExpandoObject` to a JSON string** via a pluggable serializer
   (`serializer` delegate).
4. **Deserializes the JSON string back** into the target type (`deserializer` delegate).

The two "Json" packages are thin adapters that wire `System.Text.Json` or
`Newtonsoft.Json` into steps 3–4 (see `FormsExtensions.cs` in each package).

### 1.2 Why this blocks the goals

| Liability | Effect on perf | Effect on AOT |
|---|---|---|
| Double conversion (object → JSON string → object) | Massive avoidable allocation (a transient JSON `string`, intermediate tokens) + UTF-16/UTF-8 round-trip cost | — |
| `ExpandoObject` + `IDictionary<string,object?>` | Heap allocation per node, boxing of every value | — |
| `PropertyInfo`/`DeclaredProperties` reflection at runtime | Per-call reflection cost | Reflection is trimming-hostile; linkers/AOT may strip metadata |
| `Activator.CreateInstance(List<>).MakeGenericType(...)` | Slow generic construction per call | Requires reflection; trimmed builds can fail |
| Newtonsoft path uses reflection-heavy JSON | Slow, allocates `JToken` trees | Not AOT/trimming-safe; explicitly discouraged for AOT |
| System.Text.Json reflection path | Allocates JSON DOM/string | Reflection-based STJ (`JsonSerializer.Deserialize(Type)` is non-AOT-safe; the source-gen `JsonTypeInfo<T>` path is required for AOT) |
| Recursion builds intermediate `List<KVP>` per indexed group | Allocations proportional to tree depth × breadth | — |
| No compiled/fast-path member setter | Every leaf set is a reflective property setter | — |

The current generator (`Arbor.ModelBinding.Generators`) does **not** help here: it only
 emits **value-object wrappers + their JSON/TypeConverters** (string/int/long). It
 generates **zero form-parsing code**.

### 1.3 Agreed design constraints (this engagement)

1. **Direct form → object binder.** No JSON intermediate at all — conceptually borrow
   ideas (attribute-driven mapping, `Utf8JsonReader`-style reader/writer models,
   source-gen entry points) but **eliminate the JSON round-trip**.
2. **Clean-slate conventions acceptable.** Existing test-encoded conventions may be
   redefined where better ones exist.
3. **Performance and AOT are co-equal primary goals.** No runtime IL emit, no
   `System.Reflection` usage on hot paths.
4. **Generic pure core + thin ASP.NET Core adapter** on top (mirrors the existing
   package split).
5. **Read-only binding** (form → object graph). Object → form is out of scope.
6. **Full type-shape matrix** must be supported: immutable-via-ctor, init-only setters,
   POCO settable props, records (class & struct, primary constructors), collections
   (`List/IList/ICollection/T[]/IEnumerable<T>`), dictionaries
   (`IDictionary<string,T>`/`Dictionary<string,T>`), `Nullable<T>` and nullable refs,
   generated value objects, and primitives (`int/long/bool/enum/DateTime/Guid/Uri/TimeSpan`).
7. **Explicit opt-in** source generation via attribute/marker on the target type
   (STJ `[JsonSerializable]`-style).
8. **Allocation-elimination bar.** Target: as fast as feasibly possible; treat it as an
   allocation-elimination exercise.

---

## 2. Convention model (what "form → object" must define)

A form binder must fix conventions for **six** things. The current behavior (encoded in
`tests/Arbor.ModelBinding.Tests.Unit/when_deserializing_tree.cs` and
`when_deserializing_nested_complex_type.cs`) is listed under *current*; clean-slate
alternatives under *candidate*.

### 2.1 Key shape

| Concern | Current | Candidate (clean-slate) |
|---|---|---|
| Flat scalar | `name=root` | same |
| Nested object | dot notation `prop.sub` | dot notation (keep) |
| Collection element | indexed `prop[0].sub` | indexed (keep) — most web-friendly |
| Deep tree | recursive `nodes[0].nodes[0].name` | supported (keep) |
| Case sensitivity | ordinal-ignore-case | **explicit**: default `OrdinalIgnoreCase`, per-property opt-out |
| Key delimiter | `.` (dot) | keep; forbid `.`/`[`/`]` inside leaf names |
| Collection of scalars | `Tags[0]=a&Tags[1]=b` or repeated `Tags=a&Tags=b` | **decide one canonical form**; allow both as input, emit one |
| Dictionary entry | not supported today | `key[]=value`? `key.field`? — pick `dict[key]` (bracket-key, non-numeric) |

**Recommendation:** keep dot + indexed-bracket; formalize `dict[key]` for dictionaries;
canonicalize repeated-key as a collection only when the target member is enumerable.

### 2.2 Value semantics

| Concern | Current | Candidate |
|---|---|---|
| `StringValues` single vs multi | single→scalar, multi→array | same; let the **target member** disambiguate |
| Empty string | bound as `""` | distinguish **absent key** vs **present-but-empty** (nullable gets `null`, string gets `""`) — current conflates these |
| `bool` | `"on"`→true (FormsExtensions converters) | explicit truthy set: `on, true, 1, yes`; everything else `false`; trim+ignore-case |
| `Nullable<T>` | via JSON | native: absent→null, present→convert |
| Enum | via JSON (string or number) | explicit: bind by name (ignore case) by default, numeric allowed behind a flag |
| `DateTimeOffset`/`DateTime` | via JSON | ISO-8601 + round-trip `"O"`; culture-invariant |
| `Guid`, `Uri`, `TimeSpan` | via JSON | `Guid.Parse`, `new Uri(...)`, `TimeSpan.Parse` invariant |

### 2.3 Constructor vs property binding

| Path | Current | Candidate |
|---|---|---|
| Immutable-via-ctor | JSON picks a ctor by arg-name matching | **source-generated ctor selector**: match parameter name ↔ incoming key (ignore-case); greediest public ctor wins; missing required arg → fail with a diagnostic |
| Init-only setters | (JSON supports via property name) | direct emit of `init` accessor call via `initHandler`/`System.Runtime.CompilerServices` |
| POCO setters | JSON | direct generated setter |
| Records (primary ctor) | JSON | treat primary-ctor params like ctor params + property setters for the rest |

### 2.4 Absence, nullability, and validation

Clean-slate decisions needed (current behavior is implicit/via JSON exceptions):

- **Absent key + non-nullable value type** (e.g. `int`): error vs default(0)? Recommend **error** (surfaces real form bugs), with `[BindRequired]`-style attribute.
- **Absent key + nullable**: `null`.
- **Present-but-empty + non-nullable value type**: error (e.g. `""` cannot be `int`).
- **Unknown key on strict type**: ignore by default; opt-in strict mode throws.
- **Index gaps** (`a[0]`, `a[2]` with no `a[1]`): error in strict, sparse-fill in permissive.

### 2.5 Naming policy

- Default: **case-insensitive ordinal** (matches current behavior + STJ `Web` defaults).
- Per-type/per-member overrides via attribute (e.g. `[BindName(SnakeCase)]`).
- Respect `[JsonPropertyName]` **and** a new `[BindName]` (prefer the form-specific one).

### 2.6 Collection construction

| Output type | Construction strategy |
|---|---|
| `T[]` | materialize to a temp buffer (ArrayPool), `ToArray()` at the end |
| `List<T>` / `IList<T>` / `ICollection<T>` / `IEnumerable<T>` | build `List<T>` directly (the concrete type) unless target is a concrete different type |
| `ImmutableArray<T>` / `IReadOnlyList<T>` | build `List<T>` then `ToImmutableArray` |
| `HashSet<T>` / `ISet<T>` | generate `HashSet<T>` construction |

> **Note:** current code `MakeGenericType(List<>)` + reflection `Add`. Source-gen replaces this with a known concrete `List<T>.Add` call.

---

## 3. Approaches surveyed

### 3.1 Approach A — Reflection cache (status quo, refined)

Keep the dynamic shape-builder, drop the JSON round-trip; cache `PropertyInfo` setters,
ctor `ParameterInfo`, and a converter-per-primitive table keyed by `Type`.

- **Perf:** moderate. Removes JSON cost but still boxes values (`object?`) through the
  setter cache, still allocates `ExpandoObject`/`Dictionary`.
- **AOT:** **unsafe.** `PropertyInfo.SetValue`/`MethodInfo.Invoke` require metadata that
  trimming removes; runtime would need `DynamicallyAccessedMembers` annotations
  everywhere, and generic `List<>` construction via `MakeGenericType` can fail in
  trimmed apps.
- **Allocation:** medium-high. No JSON string, but keeps the `ExpandoObject` graph.
- **Fit with constraints:** fails AOT requirement. ❌

### 3.2 Approach B — Expression-tree / Delegate cache (runtime compile)

At first use of `T`, build a typed delegate `Func<FormDataReader, T>` using
`Expression.Lambda` / `Expression.New` / `Expression.Bind`/`Expression.Assign`,
compile to a delegate, cache by `Type`. (Classic "compiled mapper".)

- **Perf:** high after warm-up (~STJ reflection-tier speed or better). Still some
  boxing at delegate boundaries unless fully typed.
- **AOT:** **unsafe.** `Expression.Compile` uses dynamic-method IL emit → rejected by
  NativeAOT. This is the same reason STJ added source-gen.
- **Allocation:** medium. Away with `ExpandoObject`; still allocates intermediate
  dictionaries unless carefully span-driven.
- **Fit:** fails AOT requirement. ❌

### 3.3 Approach C — Source-generated typed binder (Roslyn source generator, **STJ-style**)

A Roslyn source generator walks types marked with `[Bindable]` (or a marker interface)
at compile time and emits, per type `T`, a strongly-typed `IFormBinder<T>`:

```csharp
// generated (illustrative)
internal sealed class FormBinder_OrderModel : IFormBinder<OrderModel>
{
    public OrderModel Bind(ref FormDataReader reader)
    {
        int id = default;
        bool gotId = false;
        …
        while (reader.MoveNext(out var key, out var values))
        {
            switch (key.Length, key[0])        // span-driven, no string allocs
            {
                case (2, 'i') when key.SequenceEqual("id"): …
            }
        }
        return new OrderModel(id, name, …);
    }
}
```

The generator also emits a registry (`FormBinderRegistry`) mapping `Type` → factory so a
non-generic runtime entry point still works in AOT (no `MakeGenericType`).

- **Perf:** **highest feasible.** Fully typed, no boxing, no reflection, no JSON.
  Keys can be matched with `ReadOnlySpan<char>` switch/lookup. Buffers via
  `ArrayPool<T>` / `stackalloc` for indices.
- **AOT:** **safe.** Everything is generated C# cooked by the compiler; nothing is
  discovered via reflection at runtime. No `Expression.Compile`. Trimmer-safe because
  the generator emits all referenced types explicitly.
- **Allocation:** **near-zero.** No intermediate JSON string, no `ExpandoObject`,
  no per-call `PropertyInfo`. Only the result object (+ the unavoidable collection
  buffers, which can be pooled). The only remaining hot allocations are the
  user-object graph itself and any `List<T>` backing arrays.
- **Fit:** satisfies both primary goals. ✅

**Generator entry point**: explicit attribute (constraint #7). The generator also
honors the value-object generator's existing partial classes (string/int/long) so the
value-object `TryParse` paths are inlined.

### 3.4 Approach D — Source-gen **reader model** mirroring `Utf8JsonReader`

Borrow STJ's `ref struct Utf8JsonReader` *shape* directly: define a
`ref struct FormDataReader` over the raw `ReadOnlySpan<byte>` (or `ReadOnlySpan<char>`)
of the *flattened* form collection, with `MoveNext` exposing `(ReadOnlySpan<char> Key,
StringValues Value)`. Generated binders consume it like STJ's `Read(ref reader)`.

This is a **refinement of C**, not a separate strategy: it prescribes the *runtime
primitive* the generated code targets, and it is what unlocks true allocation-elimination
(span-based key compare, no `string.Split`, no `Substring`).

- **Perf:** ceiling. Form data is inherently token-izable in one pass; a span reader
  lets the binder do a single linear scan with `SequenceEqual` switches.
- **AOT:** safe (same as C).
- **Allocation:** minimal. `StringValues` is already a struct wrapping 0–N strings, so
  multi-values need no boxing. Indices can live in a `stackalloc Span<int>` for small
  depths, falling back to `ArrayPool`.
- **Fit:** ✅✅ — recommended shape.

### 3.5 Approach E — `System.Text.Json` source-gen with a *JSON shim* (rejected by constraints)

One could keep the JSON round-trip but switch to STJ's **source-generated**
`JsonTypeInfo<T>` (AOT-safe) and a hand-written form→JSON token writer that writes
directly into `Utf8JsonWriter` over a `ArrayPool`-backed `Stream` — eliminating the
JSON *string* but not the JSON *token model*.

- **Perf:** high but strictly worse than C/D (extra tokenization + UTF-8 encoding/
  decoding round-trip the binder doesn't need).
- **AOT:** safe (uses STJ source-gen).
- **Fit:** violates constraint #1 ("no JSON intermediate, conceptually borrow ideas
  only"). ❌ — listed for completeness, explicitly out of scope.

### 3.6 Approach F — Compile-time data-driven via `IncrementalGenerator` + cached `FOR` execution

Same as C but using the modern `IIncrementalGenerator` API (the current generator uses
the older `ISourceGenerator` + Scriban). The incremental pipeline gives caching by
syntax-tree, so editing a single file doesn't re-emit all binders. Templating can still
be Scriban or pure code-builder (`SyntaxFactory`).

- **Perf/AOT:** identical runtime characteristics to C.
- **Tooling:** better IDE responsiveness under large solutions; aligns with the
  direction of the .NET SDK and STJ's own generator.
- **Fit:** recommended **packaging** choice layered on top of C/D.

### 3.7 Summary matrix

| Approach | Perf | AOT-safe | Allocs | JSON? | Verdict |
|---|---|---|---|---|---|
| A. Reflection cache | medium | ❌ | medium-high | no | rejected (AOT) |
| B. Expression-tree cache | high | ❌ | medium | no | rejected (AOT) |
| C. Source-gen typed binder | **highest** | ✅ | **near-zero** | no | ✅ |
| D. C + span `FormDataReader` | **ceiling** | ✅ | **minimal** | no | ✅✅ recommended |
| E. STJ source-gen + JSON shim | high | ✅ | low-medium | **yes** | rejected (scope) |
| F. IncrementalGenerator packaging | = C/D | ✅ | = C/D | no | ✅ (adopt as packaging) |

---

## 4. Recommended design (in principle)

### 4.1 Layering (mirrors existing project layout)

```
Arbor.ModelBinding.Core                   — runtime: FormDataReader, IFormBinder<T>,
                                           FormBinderRegistry, core attributes, primitives
Arbor.ModelBinding.Generators             — IIncrementalGenerator emitting IFormBinder<T>
                                           per [Bindable] type + registry entries
Arbor.ModelBinding.AspNetCore             — thin adapter: IFormCollection → FormDataReader,
                                           ModelBinderProvider, minimal-API binder
(+ existing) Arbor.ModelBinding.Primitives — value objects: generator learns their TryParse
```

### 4.2 Source-generator entry point (constraint #7)

```csharp
[Bindable]                       // or implement marker IFormBindable
public sealed partial class OrderModel { … }

// or, STJ-style context:
[FormBinderContext]
public static partial class AppBinders
{
    public static partial FormBinderRegistry Registry { get; }
}
```

Generator emits, per type:
- a `sealed class FormBinder_OrderModel : IFormBinder<OrderModel>`,
- an entry in the generated registry,
- diagnostics for unsupported shapes (e.g. abstract target, missing required ctor arg,
  unsupported collection type).

### 4.3 Runtime core

- `ref struct FormDataReader` over the flat pair collection: one pass, span-key compare.
- `IFormBinder<T>.Bind(ref FormDataReader)` — the contract generated code implements.
- `FormBinderRegistry.TryGet(Type, out IFormBinder)` — the only non-generic escape
  hatch (used by ASP.NET adapter when `T` is known only at runtime). The registry is a
  generated `Dictionary<Type, IFormBinder>` populated from the generator's type list —
  **no runtime reflection**.
- Primitive converters as `static` typed methods (`int ParseInt(ReadOnlySpan<char>)`,
  etc.) — span-based, culture-invariant, inlined into generated binders.

### 4.4 Where the allocation-elimination comes from (constraint #8)

1. No JSON string (removes the dominant current allocation).
2. No `ExpandoObject`/`IDictionary` mirror.
3. No `PropertyInfo`/`Activator`; generated typed setters / constructor calls.
4. Span-based key matching — no `string.Split`, no `Substring`, no `StartsWith` allocs.
5. `ArrayPool<T>` / `stackalloc` for collection index buffers and temporary lists; only
   the final result arrays are allocated.
6. `StringValues` consumed without enumerating to `string[]` unless the target truly
   needs an array.

### 4.5 Performance bar to hit (constraint #8)

Existing benchmarks (`benchmarks/Arbor.ModelBinding.Benchmarks/ParsingBenchmarks.cs`)
compare Newtonsoft vs STJ on a 6-field model. The recommended design should:

- be **memory-zero** extra bytes outside the result graph (verified by
  `[MemoryDiagnoser]` showing only the expected `OrderModel` + `Tags` array + `Details`
  allocations),
- be a **multiple faster** than the STJ baseline on the same model (target: ≥10×; the
  JSON round-trip alone is ~an order of magnitude of removable work),
- scale linearly with field count and (mostly) with tree depth, not breadth.

> The existing benchmark harness already has `MemoryDiagnoser` and a representative
> target; it can be extended (add a `ParseWithSourceGen` column) to make the bar
> measurable.

### 4.6 AOT posture

- Generator is `netstandard2.0` (current), so it works for any consuming TFM.
- Runtime core stays multi-target (`netstandard2.0;net462;net8.0;net9.0` — current)
  so span APIs must be polyfilled for ns2.0 (the repo already targets ns2.0, so
  `Span<T>`/`MemoryExtensions` availability via `System.Memory` is a known cost).
  **AOT is meaningful only on net8/net9**; ns2.0/net4x remain reflection-free but
  aren't NativeAOT targets. The report should state this TFM split explicitly.
- No `[DynamicallyAccessedMembers]` needed anywhere: the generator emits every type
  the runtime touches, so there is no member-lookup to annotate.

### 4.7 Relation to the existing value-object generator

Today the generator emits `ValueObjectBase<T>` wrappers + JSON/TypeConverters for
primitive-backed strong types (`string`/`int`/`long`). The new form binder should:

- Recognize `[Bindable]` value objects and inline their generated `TryParse` rather than
  going through a converter — keeps the allocation-elimination win.
- Leave the existing value-object generator untouched (additive, not a rewrite) — the
  new binder generation is a **separate generator** or a separateemit phase.

---

## 5. Risks & open questions (to resolve before coding)

1. **ns2.0 span story.** `Span<T>` on netstandard2.0 requires the `System.Memory`
   polyfill package and forces `unsafe`-flavored APIs on net4x. Decide whether the
   allocation-elimination benefits are reserved for net8/net9 and ns2.0 gets a
   slower (but still reflection-free) path.
2. **Constructor selection rule.** "Greediest public ctor whose params all have a key"
   can silently pick surprising ctors on types with multiple ctors. Need an explicit
   attribute (`[BindConstructor]`) to pin one, with diagnostics for ambiguity.
3. **Repeated-key vs indexed collection ambiguity.** `tags=a&tags=b` into `string[] Tags`
   and `tags[0]=a&tags[1]=b` must produce identical results; mixing the two forms in one
   request must be defined (error or merged).
4. **Dictionary key encoding.** `dict[key]` needs an escaping rule for keys containing
   `]`, `[`, `.`. Decide (URL-encode? disallow?).
5. **Validation hooks.** Whether to call `System.ComponentModel.DataAnnotations`
   validators during binding, or leave to ASP.NET pipeline (recommend: leave to
   ASP.NET; binder is pure).
6. **Error reporting API.** A `FormBindResult` (Ok/Error + list of `{Key, Message}`)
   vs throwing on first error. Recommend an explicit result type so ASP.NET can map to
   `ModelStateDictionary` without exceptions.
7. **Generator identity.** Keep the existing `ISourceGenerator`+Scriban generator and
   add a new `IIncrementalGenerator` (two generators in one assembly is allowed), or
   migrate the value-object generator to incremental at the same time?
8. **Backward-compat shim.** Even with clean-slate conventions, provide a `LegacyMode`
   flag that restores current semantics (bool-from-`on`, case-insensitive, absent→default)
   so existing users can migrate incrementally? (Decision rests on whether the library
   considers itself pre-1.0 stable.)

---

## 6. Recommendation

Adopt **Approach D** (source-generated typed binder over a span-based
`FormDataReader`), packaged as an **`IIncrementalGenerator`** (Approach F packaging),
with the **layering in §4.1** and the **explicit opt-in attribute entry point** in
§4.2. This is the only surveyed option that satisfies **both** primary constraints —
high performance (near-zero allocation, no JSON round-trip, span-driven key matching)
**and** AOT safety (no reflection, no `Expression.Compile`, all type info emitted at
compile time) — while covering the full required type-shape matrix (§1.3 item 6) and
the agreed clean-slate convention model (§2).

Concrete next step (out of scope for this report, but the natural follow-up): produce a
detailed design spec for the generator contract — `IFormBinder<T>`,
`FormDataReader`, `[Bindable]`, registry emission, and the diagnostics surface — and a
single-type vertical slice (flat POCO + one indexed collection) to validate the
allocation target against the existing benchmark harness before generalizing.

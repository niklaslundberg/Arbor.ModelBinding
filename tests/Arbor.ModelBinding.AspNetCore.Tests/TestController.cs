using Arbor.ModelBinding.AspNetCore.Tests.CodeGenParsers;
using Microsoft.AspNetCore.Mvc;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("/{id}")]
        public IActionResult Get([FromRoute] MyValueObject? id)
        {
            if (id is null)
            {
                return StatusCode(500);
            }

            return Ok(id.Value);
        }

        [HttpGet]
        [Route("/typeconverter/{id}")]
        public IActionResult Get([FromRoute] SimpleValue? id)
        {
            if (id is null)
            {
                return StatusCode(500);
            }

            return Ok(id.Value);
        }

        [HttpGet]
        [Route("/typeconvertergenerated/{id}")]
        public IActionResult Get2([FromRoute] Partial1Parser? id)
        {
            if (id is null)
            {
                return StatusCode(500);
            }

            return Ok(id);
        }

        [HttpPost]
        [Route("/typeconvertergenerated/")]
        public IActionResult Post([FromBody] PostObject? instance)
        {
            if (instance is null)
            {
                return StatusCode(500);
            }

            return Ok(instance.Value);
        }
    }
}
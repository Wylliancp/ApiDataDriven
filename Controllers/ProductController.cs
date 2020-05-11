using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiDataDriven.Models;
using ApiDataDriven.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ApiDataDriven.Controllers
{


    [Route("v1/products")]
    public class ProductController : Controller
    {
        
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get(
        [FromServices] DataContext context)
        {
            return Ok(await context.Products.
                                    Include(x => x.Category).
                                    AsNoTracking().
                                    ToListAsync());
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(
            int id,
            [FromServices]DataContext context)
        {
            return Ok(await context.Products.
                                    Include(x => x.Category).
                                    AsNoTracking().
                                    FirstOrDefaultAsync(x => x.Id == id));
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Product>>> Post(
            [FromBody]Product product,
            [FromServices]DataContext context )
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                context.Products.Add(product);
                await context.SaveChangesAsync();
                
                return Ok(product);
            }
            catch
            {
                return BadRequest(new {Message = "Não foi possivel cadastrar o Produto"});
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Product>>> Put(
        int id,
        [FromBody] Product product,
        [FromServices] DataContext context)
        {
            if(id != product.Id)
                return NotFound(new {Message = "Produto invalido!"});

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                context.Entry<Product>(product).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new {Message = "Produto já foi atualizado"});
            }
            catch (Exception)
            {
                return BadRequest(new { Message = "Não foi possivel atualizar o produto!"});
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Product>>> Delete(
            int id,
            [FromServices] DataContext context)
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if(product == null)
                return NotFound(new { Message = "Produto não Encontrado"});
            
            try
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();

                return Ok(new {Message = "produto excluído com sucesso!"});
            }
            catch(Exception)
            {
                return BadRequest(new {Message = "Não foi possivel excluir o produto"});
            }
        }  
    }
}
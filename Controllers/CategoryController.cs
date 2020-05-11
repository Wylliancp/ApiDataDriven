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
    [Route("v1/categories")]
    public class CategoryController : Controller
    {
        
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        //[ResponseCache(VaryByHeader = "User-Agent", location = ResponseCacheLocation.Any, Duration = 30)]
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //Opção 2 e caso tenha uma configuraçãp global "Services.AddResponseCaching" e esta linha fala que este metodo não terá cache.
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices]DataContext context
        )
        {
            return Ok(await context.Categories.AsNoTracking().ToListAsync());
        }

        [HttpGet]
        [Route("{id:int}")] //restrição de Rota, para evitar de ficar verificando se e caracter oi inteiro
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
        int id,
        [FromServices] DataContext context)
        {
            return Ok(await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id));
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Post(
            [FromBody] Category model,
            [FromServices] DataContext context)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {
                return BadRequest(new {Message = "Não foi possível criar a categoria"}); 
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Put(
        int id, 
        [FromBody]Category model,
        [FromServices] DataContext context)
        {
            try
            {
                if(model.Id != id)
                    return NotFound(new { Message = "Categoria não encontrada"});

                if(!ModelState.IsValid)
                    return BadRequest(ModelState);

                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new {Message = "Este registro já foi atualizado"}); 
            }
            catch (Exception)
            {
                return BadRequest(new {Message = "Não foi possível atualizar a categoria"}); 
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Delete(
            int id,
            [FromServices]DataContext context
        )
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if(category == null)
                return NotFound(new {Message = "Categoria não encontrada"});

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new { Message = "Categoria Removida com sucesso."});
            }
            catch(Exception)
            {
                return BadRequest(new { Message = "Não foi possivel remover a category"});
            }
        }
    }
}
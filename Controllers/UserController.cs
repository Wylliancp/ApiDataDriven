using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiDataDriven.Data;
using ApiDataDriven.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using ApiDataDriven.Services;

namespace ApiDataDriven.Controllers
{
    [Route("v1/users")]
    public class UserController : Controller
    {

    [HttpGet]
    [Route("")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<List<User>>> Get(
        [FromServices] DataContext context
    )
    {
        var result = await context.Users.AsNoTracking().ToListAsync();
        return Ok(result);
    }

     [HttpPost]
     [Route("")]
     [AllowAnonymous]
    //[Authorize(Roles = "manager")]
     public async Task<ActionResult<User>> Post(
        [FromServices] DataContext context,
        [FromBody] User user)   
     {
         //Verifica se os dados são válidos
         if(!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            user.Password = "";
            return user;
        }
        catch (Exception)
        {
            return BadRequest(new {Message = "Não foi possivel criar o Usuário"});
        }
     }

     [HttpPost]
     [Route("login")]
     public async Task<ActionResult<dynamic>> Authenticate(
         [FromServices] DataContext context,
         [FromBody] User model)
      {
       var user = await context.Users.AsNoTracking().Where(x => x.Username == model.Username
                                                            && x.Password == model.Password)
                                                            .FirstOrDefaultAsync();

        if(user == null)
            return NotFound(new {Message = "Usúario ou senha inválido"});

        var token = TokenServices.GenerateToken(user);
        user.Password = "";
        return new  
        {
            user = user,
            token = token
        };     
      }

      //testes
      [HttpGet]
      [Route("anonimo")]
      [AllowAnonymous]
      public string Anonimo() => "Anonimo";
      [HttpGet]
      [Route("autenticado")]
      [Authorize]
      public string Autenticado() => "Autenticado";
      [HttpGet]
      [Route("funcionario")]
      [Authorize(Roles = "employee")]
      public string funcionario() => "funcionario";
      [HttpGet]
      [Route("gerente")]
      [Authorize(Roles = "manager")]
      public string Gerente() => "Gerente";
    }
}
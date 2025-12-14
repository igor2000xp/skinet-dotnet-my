using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.Specifications;
// using System.Linq;
// using API.RequestHelpers;
// using Core.Entities;
// using Core.Interfaces;
// using Core.Specifications;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IGenericRepository<Product> repo) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
    {
        var spec = new ProductSpecification(brand, type, sort);
        var products = await repo.ListAsync(spec);

        return Ok(products);
        // Implementation to retrieve products
        // return Ok(new[] { "Product1", "Product2" });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);
        if (product == null) return NotFound();
        
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repo.Add(product);
        if(await repo.SaveAllAsync())
        {
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }
        // return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        return BadRequest("Problem creating product");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id || !ProductExist(id)) return BadRequest("Invalid product ID"); 
        // context.Entry(product).State = EntityState.Modified;
        repo.Update(product);

        if (await repo.SaveAllAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem updating product");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);
        if (product == null) return NotFound();

        repo.Remove(product);
        if (await repo.SaveAllAsync())
        {
            return NoContent();
        }
        return BadRequest("Problem deleting product");
    }
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        // return Ok(await repo.GetProductBrandsAsync());
        var spec = new BrandListSpecification();
        return Ok(await repo.ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes() 
    {
        // return Ok(await repo.GetProductTypesAsync());
        var spec = new TypeListSpecification();
        return Ok(await repo.ListAsync(spec));
    }

    private bool ProductExist(int id)
    {
        return repo.Exists(id);
    }
}

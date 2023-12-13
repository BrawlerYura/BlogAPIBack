using System.Net;
using BlogApi.Data;
using BlogApi.Data.Models;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/address")]
public class AddressController
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }
    
    [HttpGet]
    [Route("search")]
    [ProducesResponseType(typeof(ExceptionDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<SearchAddressModel> SearchAddress([FromQuery] int parentObjectId, [FromQuery] string query)
    {
        return await _addressService.SearchAddress(parentObjectId, query);
    }
    
    [HttpGet]
    [Route("chain")]
    [ProducesResponseType(typeof(ExceptionDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<SearchAddressModel> ChainAddress([FromQuery] Guid objectGuid)
    {
        return await _addressService.ChainAddress(objectGuid);
    }
}
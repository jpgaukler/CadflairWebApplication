using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairDataAccess.Services;
using Microsoft.AspNetCore.Mvc;

namespace CadflairRestApi.Controllers;

[ApiController]
[Route("api/v1")]
public class McMasterController : ControllerBase
{

    private readonly McMasterService _mcMasterService;
    private readonly ILogger<McMasterController> _logger;

    public McMasterController(DataServicesManager dataServicesManager, ILogger<McMasterController> logger) 
    {
        _mcMasterService = dataServicesManager.McMasterService;
        _logger = logger;
    }

    #region "Category"

    //[HttpPost]
    //[Route("categories")]
    //public Task<Category> CreateCategory(int subscriptionId, int? parentId, string name, string description, string thumbnailUri, int createdById)
    //{

    //}

    [HttpGet]
    [Route("categories/{id}")]
    public async Task<ActionResult<Category>> GetCategoryById(int id)
    {
        try
        {
            var result = await _mcMasterService.GetCategoryById(id);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred!");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred!");
        }
    }

    [HttpGet]
    [Route("subscriptions/{subscriptionId}/categories")]
    public async Task<ActionResult<List<Category>>> GetCategoriesBySubscriptionId(int subscriptionId)
    {
        try
        {
            var result = await _mcMasterService.GetCategoriesBySubscriptionId(subscriptionId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred!");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred!");
        }
    }

    //[HttpPut]
    //public Task UpdateCategory(Category category)
    //{
    //}

    //[HttpDelete]
    //public Task DeleteCategoryById(int categoryId)
    //{
    //}

    #endregion

    #region "ProductDefinition"

    //[HttpPost]
    //public async Task<ProductDefinition> CreateProductDefinition(int subscriptionId, int? categoryId, string name, string description, string thumbnailUri, string forgeBucketKey, int createdById)
    //{
    //}

    [HttpGet]
    [Route("subscriptions/{subscriptionId}/product-definitions")]
    public async Task<ActionResult<List<ProductDefinition>>> GetProductDefinitionsBySubscriptionId(int subscriptionId)
    {
        try
        {
            var result = await _mcMasterService.GetProductDefinitionsBySubscriptionId(subscriptionId);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred!");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred!");
        }
    }

    [HttpGet]
    [Route("subscriptions/{subscriptionId}/product-definitions/{name}")]
    public async Task<ActionResult<ProductDefinition>> GetProductDefinitionByNameAndSubscriptionId(string name, int subscriptionId)
    { 
        try
        {
            var result = await _mcMasterService.GetProductDefinitionByNameAndSubscriptionId(name, subscriptionId);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred!");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred!");
        }
    }


    //[HttpPut]
    //public Task UpdateProductDefinition(ProductDefinition productDefinition)
    //{
    //}

    //[HttpDelete]
    //public Task DeleteProductDefinitionById(int productDefinitionId)
    //{
    //}

    #endregion

    #region "ProductTable"

    //[HttpPost]
    //public Task<ProductTable> CreateProductTable(int productDefinitionId, int createdById)
    //{
    //}

    [HttpGet]
    [Route("product-definitions/{productDefinitionId}/product-table")]
    public async Task<ActionResult<ProductTable>> GetProductTableByProductDefinitionId(int productDefinitionId)
    {
        try
        {
            var result = await _mcMasterService.GetProductTableByProductDefinitionId(productDefinitionId);
            return result == null ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred!");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred!");
        }
    }

    //[HttpDelete]
    //public Task DeleteProductTableById(int productTableId)
    //{
    //}


    #endregion

    #region "Column"

    //[HttpPost]
    //public Task<Column> CreateColumn(int productTableId, string header, int sortOrder, int createdById)
    //{
    //}


    //[HttpPut]
    //public Task UpdateColumn(Column column)
    //{
    //}

    //[HttpDelete]
    //public Task DeleteColumnById(int columnId)
    //{
    //}

    #endregion

    #region "Row"

    //[HttpPost]
    //public Task<Row> CreateRow(int productTableId, string partNumber, int createdById)
    //{
    //}

    //[HttpPut]
    //public async Task UpdateRow(Row row)
    //{
    //}

    //[HttpDelete]
    //public Task DeleteRowById(int rowId)
    //{
    //}


    #endregion

    #region "TableValue"

    //[HttpPost]
    //public Task<TableValue> CreateTableValue(int productTableId, int columnId, int rowId, string value, int createdById)
    //{
    //}

    #endregion

    #region "Attachment"

    //[HttpPost]
    //public Task<Attachment> CreateAttachment(int rowId, string forgeObjectKey, int createdById)
    //{
    //}

    #endregion

}


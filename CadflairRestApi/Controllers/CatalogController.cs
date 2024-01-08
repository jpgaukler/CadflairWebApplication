using CadflairDataAccess;
using CadflairDataAccess.Models;
using CadflairDataAccess.Services;
using Microsoft.AspNetCore.Mvc;

namespace CadflairRestApi.Controllers;

[ApiController]
[Route("api/v1")]
public class CatalogController : ControllerBase
{

    private readonly CatalogService _mcMasterService;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(DataServicesManager dataServicesManager, ILogger<CatalogController> logger) 
    {
        _mcMasterService = dataServicesManager.CatalogService;
        _logger = logger;
    }

    #region "Category"

    //[HttpPost]
    //[Route("categories")]
    //public Task<Category> CreateCategory(int subscriptionId, int? parentId, string name, string description, string thumbnailUri, int createdById)
    //{

    //}


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

    [HttpGet]
    [Route("subscriptions/{subscriptionId}/categories/{categoryName}")]
    public async Task<ActionResult<List<Category>>> GetCategoriesBySubscriptionIdAndCategoryName(int subscriptionId, string categoryName)
    {
        try
        {
            var allCategories = await _mcMasterService.GetCategoriesBySubscriptionId(subscriptionId);
            var category = allCategories.ToFlatList().FirstOrDefault(i => i.Name == categoryName);
            return category == null ? NotFound() : Ok(category);
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
            var productDefinition = await _mcMasterService.GetProductDefinitionByNameAndSubscriptionId(name, subscriptionId);
            if (productDefinition == null)
                return NotFound();

            var productTable = await _mcMasterService.GetProductTableByProductDefinitionId(productDefinition.Id);
            return Ok(new { productDefinition, productTable });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred!");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred!");
        }
    }

    [HttpGet]
    [Route("subscriptions/{subscriptionId}/categories/{categoryId}/product-definitions")]
    public async Task<ActionResult<List<ProductDefinition>>> GetProductDefinitionsBySubscriptionIdAndCategoryId(int subscriptionId, int categoryId)
    {
        try
        {
            var allProductDefinitions = await _mcMasterService.GetProductDefinitionsBySubscriptionId(subscriptionId);
            return Ok(allProductDefinitions.Where(i => i.CategoryId == categoryId).ToList());
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


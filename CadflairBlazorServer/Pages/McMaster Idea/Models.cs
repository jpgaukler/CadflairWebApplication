namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProductCategory? ParentCategory { get; set; } 

        public ThumbnailImage ThumbnailImage { get; set; } = new();


        /// <summary>
        /// There may be sub categories for further grouping.
        /// </summary>
        public List<ProductCategory> ChildCategories { get; set; } = new();
        public List<ProductDefinition> ProductDefinitions { get; set; } = new();
    }

    public class ThumbnailImage
    {
        public int Id { get; set; }
        public string Base64String { get; set; } = string.Empty;
        public byte[] Bytes { get; set; } 
    }

    public class ProductDefinition
    {
        public int Id { get; set; }
        public int ProductCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ThumbnailImage ThumbnailImage { get; set; } = new();
        public List<ColumnDefinition> ColumnDefinitions { get; set; } = new();
    }

    public class ColumnDefinition
    {
        public int Id { get; set; }
        public int ProductDefinitionId { get; set; }
        public string Header { get; set; } = string.Empty;
        //public ValueTypeEnum ValueTypeEnum { get; set; }
        public bool IsVisible { get; set; }
        public bool IsFilterable { get; set; }
        //public object ControlType { get; set; }
    }

    public class ColumnValue
    {
        public int Id { get; set; }
        public int ColumnDefinitionId { get; set; }
        //public ValueTypeEnum ValueTypeEnum { get; set; }
        public string Value { get; set; } = string.Empty;
        public ThumbnailImage ThumbnailImage { get; set; } = new(); 
    }

    public class Product
    {
        public int Id { get; set; }
        public int ProductDefinitionId { get; set; }
        public List<ColumnValue> ColumnValues { get; set; } = new();
        public string BucketKey { get; set; } = string.Empty;
        public string ObjectKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// This is just a join table
    /// </summary>
    public class ProductColumnValue
    {
        public int ProductRecordId { get; set; }
        public int ColumnValueId { get; set; }
    }

    public enum ValueTypeEnum : int
    {
        Boolean = 1,
        Double = 2,
        Integer = 3,
        String = 3,
        Decimal = 3,
    }

}

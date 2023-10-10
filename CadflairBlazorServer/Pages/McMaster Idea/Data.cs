namespace CadflairBlazorServer.Pages.McMaster_Idea
{
    public static class DummyData
    {
        private static List<ProductCategory> _productCategories = new()
        {
            new ProductCategory()
            {
                Name = "Flanges",
                Description = "This is a flange that is great for general use.",
                ChildCategories = new()
                {
                    new ProductCategory()
                    {
                        Name = "Stainless Steel Flanges",
                        Description = "This is a flange that is great for corrosion resistance.",
                        ProductDefinitions = new()
                        {
                            new ProductDefinition()
                            {
                                Id = 1,
                                Name = "304 Standard Corrosion Resistant Blind Flange",
                                Description = "This is a flange that is coated in black oxide for mild corrosion resistance.",
                                ColumnDefinitions = new()
                                {
                                    new ColumnDefinition()
                                    {
                                        Id = 1,
                                        Header = "Diameter",
                                        IsFilterable = true,
                                        IsVisible = true,
                                    },
                                    new ColumnDefinition()
                                    {
                                        Id = 2,
                                        Header = "Length",
                                        IsFilterable = true,
                                        IsVisible = true,
                                    },
                                    new ColumnDefinition()
                                    {
                                        Id = 3,
                                        Header = "Bolt Hole Qty",
                                        IsFilterable = true,
                                        IsVisible = true,
                                    },
                                }
                            },
                            new ProductDefinition()
                            {
                                Id = 2,
                                Name = "316 Super Corrosion Resistant Blind Flange",
                                Description = "This is a flange that is coated in black oxide for mild corrosion resistance.",
                                ColumnDefinitions = new()
                                {
                                    new ColumnDefinition()
                                    {
                                        Id = 4,
                                        Header = "Diameter",
                                        IsFilterable = true,
                                        IsVisible = true,
                                    },
                                    new ColumnDefinition()
                                    {
                                        Id = 5,
                                        Header = "Length",
                                        IsFilterable = true,
                                        IsVisible = true,
                                    },
                                    new ColumnDefinition()
                                    {
                                        Id = 6,
                                        Header = "Bolt Hole Qty",
                                        IsFilterable = true,
                                        IsVisible = true,
                                    },
                                }
                            },
                        }
                    },
                    new ProductCategory()
                    {
                        Id = 3,
                        Name = "Carbon Steel Flanges",
                        Description = "This is a flange that is poor for corrosion resistance.",
                        ProductDefinitions = new()
                        {
                            new ProductDefinition()
                            {
                                Id = 3,
                                Name = "Black Oxide Blind Flange",
                                Description = "This is a flange that is coated in black oxide for mild corrosion resistance.",
                                ColumnDefinitions = new()
                                {
                                    new ColumnDefinition()
                                    {
                                        Id = 7,
                                        Header = "Diameter",
                                        IsFilterable = true,
                                        IsVisible = true,
                                    },
                                    new ColumnDefinition()
                                    {
                                        Id = 8,
                                        Header = "Length",
                                        IsFilterable = true,
                                        IsVisible = true,
                                    },
                                    new ColumnDefinition()
                                    {
                                        Id = 9,
                                        Header = "Bolt Hole Qty",
                                        IsFilterable = true,
                                        IsVisible = true,
                                    },
                                }
                            },
                            new ProductDefinition()
                            {
                                Id = 4,
                                Name = "Black Oxide Raised Flange",
                                Description = "This is a flange that is coated in black oxide for mild corrosion resistance.",
                                ColumnDefinitions = new()
                                {
                                    new ColumnDefinition()
                                    {
                                        Id = 10,
                                        Header = "Diameter",
                                        IsFilterable = true,
                                        IsVisible = true,
                                    },
                                    new ColumnDefinition()
                                    {
                                        Id = 11,
                                        Header = "Length",
                                        IsFilterable = true,
                                        IsVisible = true,
                                    },
                                    new ColumnDefinition()
                                    {
                                        Id = 12,
                                        Header = "Bolt Hole Qty",
                                        IsFilterable = true,
                                        IsVisible = true,
                                    },
                                }
                            },
                        }
                    },
                }
            },
        };

        private static List<Product> _product1 = new()
        {
            new Product()
            {
                Id = 1,
                ProductDefinitionId = 1,
                BucketKey = "b6f44344-ef76-46ee-82be-904077dddcaf",
                ObjectKey = "AWWA C207-18 Class B Size 4 Flange.stp",
                ColumnValues = new()
                {
                    new ColumnValue() { ColumnDefinitionId = 1, Value = "1" },
                    new ColumnValue() { ColumnDefinitionId = 2, Value = "10" },
                    new ColumnValue() { ColumnDefinitionId = 3, Value = "6" },
                }
            },
            new Product()
            {
                Id = 2,
                ProductDefinitionId = 1,
                BucketKey = "3c8f29dd-9945-4f67-9c68-2ba2b62c36b4",
                ObjectKey = "AWWA C207-18 Class B Size 5 Flange.stp",
                ColumnValues = new()
                {
                    new ColumnValue() { ColumnDefinitionId = 1, Value = "2" },
                    new ColumnValue() { ColumnDefinitionId = 2, Value = "30" },
                    new ColumnValue() { ColumnDefinitionId = 3, Value = "8" },
                }
            },
            new Product()
            {
                Id = 1,
                ProductDefinitionId = 1,
                BucketKey = "35b8c9ed-008a-40b1-b409-9686f3cb1e5f",
                ObjectKey = "AWWA C207-18 Class B Size 6 Flange.stp",
                ColumnValues = new()
                {
                    new ColumnValue() { ColumnDefinitionId = 1, Value = "3" },
                    new ColumnValue() { ColumnDefinitionId = 2, Value = "40" },
                    new ColumnValue() { ColumnDefinitionId = 3, Value = "12" },
                }
            },
            new Product()
            {
                Id = 4,
                ProductDefinitionId = 1,
                BucketKey = "0bbdb236-b124-45ec-aefc-c9af56197424",
                ObjectKey = "AWWA C207-18 Class B Size 60 Flange.stp",
                ColumnValues = new()
                {
                    new ColumnValue() { ColumnDefinitionId = 1, Value = "3" },
                    new ColumnValue() { ColumnDefinitionId = 2, Value = "30" },
                    new ColumnValue() { ColumnDefinitionId = 3, Value = "12" },
                }
            },
            new Product()
            {
                Id = 5,
                ProductDefinitionId = 1,
                BucketKey = "3895b891-8833-4f42-9e02-f54978efd918",
                ObjectKey = "AWWA C207-18 Class B Size 66 Flange.stp",
                ColumnValues = new()
                {
                    new ColumnValue() { ColumnDefinitionId = 1, Value = "3" },
                    new ColumnValue() { ColumnDefinitionId = 2, Value = "50" },
                    new ColumnValue() { ColumnDefinitionId = 3, Value = "12" },
                }
            },

        };

        public static List<ProductCategory> GetProductCategories()
        {
            return _productCategories;
        }

        public static List<Product> GetProductByProductDefinitionId(int productDefinitionId)
        {
            if (productDefinitionId == 1)
                return _product1;

            return new List<Product>();
        }

    }
}

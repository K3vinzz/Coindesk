## 實作加分項目 
1. Error handling 處理 API response  
2. swagger-ui  
3. 能夠運行在 Docker  

## 建立Table
```SQL
CREATE TABLE [Currencies] (
          [Id] int NOT NULL IDENTITY,
          [Code] nvarchar(max) NOT NULL,
          [Name] nvarchar(max) NULL,
          CONSTRAINT [PK_Currencies] PRIMARY KEY ([Id])
      );
```

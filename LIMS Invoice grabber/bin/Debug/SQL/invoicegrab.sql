declare @List TABLE
    (
        Value varchar(30)
    )
    DECLARE
        @Value varchar(30),
        @Pos int

    SET @accountnumber = LTRIM(RTRIM(@accountnumber))+ ','
    SET @Pos = CHARINDEX(',', @accountnumber, 1)

    IF REPLACE(@accountnumber, ',', '') <> ''
    BEGIN
        WHILE @Pos > 0
        BEGIN
            SET @Value = LTRIM(RTRIM(LEFT(@accountnumber, @Pos - 1)))

            IF @Value <> ''
                INSERT INTO @List (Value) VALUES (@Value) 

            SET @accountnumber = RIGHT(@accountnumber, LEN(@accountnumber) - @Pos)
            SET @Pos = CHARINDEX(',', @accountnumber, 1)
        END
    END     

declare @InvoiceList TABLE
    (
        Value varchar(30)
    )
    DECLARE
        @Value2 varchar(30),
        @Pos2 int

    SET @invoicenumber = LTRIM(RTRIM(@invoicenumber))+ ','
    SET @Pos2 = CHARINDEX(',', @invoicenumber, 1)

    IF REPLACE(@invoicenumber, ',', '') <> ''
    BEGIN
        WHILE @Pos2 > 0
        BEGIN
            SET @Value = LTRIM(RTRIM(LEFT(@invoicenumber, @Pos2 - 1)))

            IF @Value <> ''
                INSERT INTO @List (Value) VALUES (@Value2) 

            SET @invoicenumber = RIGHT(@invoicenumber, LEN(@invoicenumber) - @Pos2)
            SET @Pos2 = CHARINDEX(',', @invoicenumber, 1)
        END
    END 

select SAMPLE.SAMPLE_NUMBER,
SAMPLE.Z_PRODUCT_LOT,
PRODUCT.DESCRIPTION, 
INVOICE_ITEM.INVOICE_ITEM_NO, 
INVOICE_ITEM.UNIT_PRICE, 
INVOICE_ITEM.COST_ITEM_NO, 
INVOICE_ITEM.QUANTITY, 
INVOICE_ITEM.TOTAL_PRICE, 
INVOICE.ITEM_TOTAL, 
INVOICE.INVOICE_NUMBER, 
INVOICE_ITEM.BILLING_ITEM_DESC, 
INVOICE.INVOICED_ON, 
CUSTOMER.COMPANY_NAME, 
CONTRACT_QUOTE.ACCOUNT_NUMBER, 
CUSTOMER.PHONE_NUM, 
CUSTOMER.Z_RECEIPT_DIST, 
CUSTOMER.CONTACT, 
PROJECT.T_PO_NUMBER, 
SAMPLE.Z_RUSH, 
SAMPLE.RECD_DATE,
SAMPLE.LOGIN_DATE
from INVOICE as INVOICE 
right outer join INVOICE_ITEM on INVOICE.INVOICE_NUMBER = INVOICE_ITEM.INVOICE_NUMBER 
left outer join Z_INVOICE_ITEM_TEST on INVOICE_ITEM.INVOICE_ITEM_NO = Z_INVOICE_ITEM_TEST.INVOICE_ITEM_NO 
left outer join CUSTOMER 
inner join SAMPLE on CUSTOMER.NAME = SAMPLE.CUSTOMER on Z_INVOICE_ITEM_TEST.SAMPLE_NUMBER = SAMPLE.SAMPLE_NUMBER 
left outer join PRODUCT on SAMPLE.PRODUCT = PRODUCT.NAME and SAMPLE.PRODUCT_VERSION = PRODUCT.VERSION 
left outer join PROJECT on SAMPLE.PROJECT = PROJECT.NAME 
left outer join CONTRACT_QUOTE on INVOICE.CONTRACT_NUMBER = CONTRACT_QUOTE.CONTRACT_QUOTE_NO 
where 
(INVOICE.INVOICE_NUMBER IN (SELECT * FROM @InvoiceList) or @invoicenumber IS NULL) 
and (@accountnumber IS NULL or CONTRACT_QUOTE.ACCOUNT_NUMBER IN (SELECT * FROM @List))
and ((SAMPLE.SAMPLE_NUMBER = @samplenum or @samplenum IS NULL) or (SAMPLE.Z_PRODUCT_LOT = @lot or @lot IS NULL))
and (INVOICED_ON >=@fromdate or @fromdate IS NULL)
and (INVOICED_ON <=@todate or @todate IS NULL)

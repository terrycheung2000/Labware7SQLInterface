/*  Created for LABWARE 7 database
	invoicepdf.sql

	Inputs: accountnumber, invoicenumber, fromdate, todate

	Output: List of invoices with corresponding path according to user search
*/

-- table variable translates account number in csv to 1 column list
declare @accountList TABLE
    (
        Value varchar(30)
    )
    DECLARE
        @Value1 varchar(30),
        @Pos1 int
	-- reads csv and records string pos
    SET @accountnumber = LTRIM(RTRIM(@accountnumber))+ ','
    SET @Pos1 = CHARINDEX(',', @accountnumber, 1)

    IF REPLACE(@accountnumber, ',', '') <> ''
    BEGIN
        WHILE @Pos1 > 0
        BEGIN
			-- sets value into list
            SET @Value1 = LTRIM(RTRIM(LEFT(@accountnumber, @Pos1 - 1)))

            IF @Value1 <> ''
                INSERT INTO @accountList (Value) VALUES (@Value1) 

            SET @accountnumber = RIGHT(@accountnumber, LEN(@accountnumber) - @Pos1)
            SET @Pos1 = CHARINDEX(',', @accountnumber, 1)
        END
    END  

-- table variable translates invoice number in csv to 1 column list
declare @invoiceList TABLE
    (
        Value varchar(30)
    )
    DECLARE
        @Value2 varchar(30),
        @Pos2 int
	-- reads csv and records string pos
    SET @invoicenumber = LTRIM(RTRIM(@invoicenumber))+ ','
    SET @Pos2 = CHARINDEX(',', @invoicenumber, 1)

    IF REPLACE(@invoicenumber, ',', '') <> ''
    BEGIN
        WHILE @Pos2 > 0
        BEGIN
			-- sets value into list
            SET @Value2 = LTRIM(RTRIM(LEFT(@invoicenumber, @Pos2 - 1)))

            IF @Value2 <> ''
                INSERT INTO @invoiceList (Value) VALUES (@Value2) 

            SET @invoicenumber = RIGHT(@invoicenumber, LEN(@invoicenumber) - @Pos2)
            SET @Pos2 = CHARINDEX(',', @invoicenumber, 1)
        END
    END 

-- selecting column values
SELECT   dbo.INVOICE.INVOICE_NUMBER, 
		dbo.INVOICE.CONTRACT_NUMBER, 
		dbo.INVOICE.CUSTOMER, 
		dbo.CONTRACT_QUOTE.ACCOUNT_NUMBER,
		dbo.INVOICE.PO_NUMBER, 
		dbo.INVOICE.ITEM_TOTAL, 
		dbo.INVOICE.PAYMENT_DUE, 
		dbo.INVOICE.INVOICE_STARTED, 
		dbo.INVOICE.INVOICE_SCHED_DATE, 
		dbo.INVOICE.INVOICED_ON,
		-- checks if email exists within Z_EMAIL_TRANSACTIONS and assigns T if list exists
		(case when EXISTS(select * from Z_EMAIL_TRANSACTIONS where Z_EMAIL_TRANSACTIONS.INVOICE_NUMBER = invoice.INVOICE_NUMBER) then 'T' else 'F' end)as 'EMAIL SENT',
		dbo.INVOICE.T_PDF_FILE
-- joining tables
FROM            dbo.INVOICE INNER JOIN
                         dbo.CONTRACT_QUOTE ON dbo.INVOICE.CONTRACT_NUMBER = dbo.CONTRACT_QUOTE.CONTRACT_QUOTE_NO
-- user search. Ignore if null
WHERE        (CONTRACT_QUOTE.ACCOUNT_NUMBER IN (SELECT * FROM @accountList) or @accountnumber IS NULL)
			and (INVOICE.INVOICE_NUMBER in (SELECT * FROM @invoiceList) or @invoicenumber IS NULL)
			and (INVOICED_ON >=@fromdate or @fromdate IS NULL)
			and (INVOICED_ON <=@todate or @todate IS NULL)
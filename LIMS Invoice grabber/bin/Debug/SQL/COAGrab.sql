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


select customer.COMPANY_NAME, z_email_transactions.SAMPLE_NAME, DATE_SENT, EMAIL_ATTACHMENT as T_PDF_FILE from Z_EMAIL_TRANSACTIONS 
inner join sample on sample.TEXT_ID = Z_EMAIL_TRANSACTIONS.SAMPLE_NAME 
inner join customer on customer.name = sample.CUSTOMER
inner join account on account.CUSTOMER = customer.NAME

where EMAIL_ATTACHMENT like '%\COA\%' 
and (account.ACCOUNT_NUMBER in (select * from @List) or @accountnumber is null) 
and ((SAMPLE.SAMPLE_NUMBER = @samplenum or @samplenum IS NULL) or (SAMPLE.Z_PRODUCT_LOT = @lot or @lot IS NULL))
and (Z_EMAIL_TRANSACTIONS.DATE_SENT >= @fromdate or @fromdate is null)
and (Z_EMAIL_TRANSACTIONS.DATE_SENT <= @todate or @todate is null)





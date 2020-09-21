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

select ANALYSIS_TYPE, 
		SAMPLE.DUE_DATE, 
		SAMPLE.SAMPLE_NUMBER, 
		SAMPLE.Z_PRODUCT_LOT, 
		TEST.STATUS as TEST_STATUS, 
		SAMPLE.STATUS as SAMPLE_STATUS,
		sample.OLD_STATUS,
		test.Z_INVESTIGATION, 
		TEST.REPORTED_NAME, 
		SAMPLE.Z_CUST_PO_NO, 
		SAMPLE.PROJECT, 
		SAMPLE.CUSTOMER, 
		TEST.DATE_RECEIVED, 
		SAMPLE.Z_RUSH
	from project 
	left join sample on project.NAME = sample.PROJECT
	left join test on sample.SAMPLE_NUMBER = test.SAMPLE_NUMBER
	left join ANALYSIS on test.ANALYSIS = ANALYSIS.NAME
	left join account on account.CUSTOMER = SAMPLE.CUSTOMER
	where active = 'T' and SAMPLE.status != 'X' and PROJECT.CLOSED != 'T' and sample.STATUS != 'A' and test.STATUS != 'X' 
	and (account.ACCOUNT_NUMBER in (select * from @List) or @accountnumber is null) 
	and ((SAMPLE.SAMPLE_NUMBER = @samplenum or @samplenum IS NULL) or (SAMPLE.Z_PRODUCT_LOT = @lot or @lot IS NULL))
	and (TEST.DATE_RECEIVED >= @fromdate or @fromdate is null)
	and (TEST.DATE_RECEIVED <= @todate or @todate is null)




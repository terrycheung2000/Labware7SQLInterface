/*  Created for LABWARE 7 database
	testsinprogress.sql

	Inputs: accountnumber, samplenumber, fromdate, todate

	Output: all active tests according to user search
*/

-- table variable translates account number in csv to 1 column list
declare @List TABLE
    (
        Value varchar(30)
    )
    DECLARE
        @Value varchar(30),
        @Pos int
	-- reading csv and records string pos
    SET @accountnumber = LTRIM(RTRIM(@accountnumber))+ ','
    SET @Pos = CHARINDEX(',', @accountnumber, 1)

    IF REPLACE(@accountnumber, ',', '') <> ''
    BEGIN
        WHILE @Pos > 0
        BEGIN
			-- sets value into list
            SET @Value = LTRIM(RTRIM(LEFT(@accountnumber, @Pos - 1)))

            IF @Value <> ''
                INSERT INTO @List (Value) VALUES (@Value) 

            SET @accountnumber = RIGHT(@accountnumber, LEN(@accountnumber) - @Pos)
            SET @Pos = CHARINDEX(',', @accountnumber, 1)
        END
    END     

-- selecting column values
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
	-- joining tables
	left join sample on project.NAME = sample.PROJECT
	left join test on sample.SAMPLE_NUMBER = test.SAMPLE_NUMBER
	left join ANALYSIS on test.ANALYSIS = ANALYSIS.NAME
	left join account on account.CUSTOMER = SAMPLE.CUSTOMER
	-- selecting only active tests 
	where active = 'T' and SAMPLE.status != 'X' and PROJECT.CLOSED != 'T' and sample.STATUS != 'A' and test.STATUS != 'X' 
	-- user search. Ignore if null
	and (account.ACCOUNT_NUMBER in (select * from @List) or @accountnumber is null) 
	and ((SAMPLE.SAMPLE_NUMBER = @samplenum or @samplenum IS NULL) or (SAMPLE.Z_PRODUCT_LOT = @lot or @lot IS NULL))
	and (TEST.DATE_RECEIVED >= @fromdate or @fromdate is null)
	and (TEST.DATE_RECEIVED <= @todate or @todate is null)




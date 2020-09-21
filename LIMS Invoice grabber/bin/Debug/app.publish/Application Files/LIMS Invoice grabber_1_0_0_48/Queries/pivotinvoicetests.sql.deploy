/*  Created for LABWARE 7 database
	pivotinvoicetests.sql

	Inputs: accountnumber, fromdate, todate

	Output: pivot table containing sum of test revenue 
	per original test name according to given time frame
	and account number
*/

-- table variable translates account number in csv to 1 column list
declare @List TABLE
    (
        Value varchar(30)
    )
    DECLARE
        @Value varchar(30),
        @Pos int
	-- reading csv and recording string pos
    SET @accountnumber = LTRIM(RTRIM(@accountnumber))+ ','
    SET @Pos = CHARINDEX(',', @accountnumber, 1)

    IF REPLACE(@accountnumber, ',', '') <> ''
    BEGIN
        WHILE @Pos > 0
        BEGIN
			-- sets value to list
            SET @Value = LTRIM(RTRIM(LEFT(@accountnumber, @Pos - 1)))

            IF @Value <> ''
                INSERT INTO @List (Value) VALUES (@Value) 

            SET @accountnumber = RIGHT(@accountnumber, LEN(@accountnumber) - @Pos)
            SET @Pos = CHARINDEX(',', @accountnumber, 1)
        END
    END 

-- table variable to store current timeframe query 
declare @invoicegrabcurr TABLE (TOTAL_PRICE real, BILLING_ITEM_DESC varchar(MAX))
insert into @invoicegrabcurr 
	-- selecting required columns 
	select  
		INVOICE_ITEM.TOTAL_PRICE, 
		INVOICE_ITEM.BILLING_ITEM_DESC
	from INVOICE as INVOICE 
		-- joining tables
		right outer join INVOICE_ITEM on INVOICE.INVOICE_NUMBER = INVOICE_ITEM.INVOICE_NUMBER 
		left outer join Z_INVOICE_ITEM_TEST on INVOICE_ITEM.INVOICE_ITEM_NO = Z_INVOICE_ITEM_TEST.INVOICE_ITEM_NO 
		left outer join CUSTOMER 
		inner join SAMPLE on CUSTOMER.NAME = SAMPLE.CUSTOMER on Z_INVOICE_ITEM_TEST.SAMPLE_NUMBER = SAMPLE.SAMPLE_NUMBER 
		left outer join PRODUCT on SAMPLE.PRODUCT = PRODUCT.NAME and SAMPLE.PRODUCT_VERSION = PRODUCT.VERSION 
		left outer join PROJECT on SAMPLE.PROJECT = PROJECT.NAME 
		left outer join CONTRACT_QUOTE on INVOICE.CONTRACT_NUMBER = CONTRACT_QUOTE.CONTRACT_QUOTE_NO 
	where 
	-- user search. Ignore if null
	(@accountnumber IS NULL or CONTRACT_QUOTE.ACCOUNT_NUMBER IN (SELECT * FROM @List))
	and (INVOICED_ON >=@fromdate or @fromdate IS NULL)
	and (INVOICED_ON <=@todate or @todate IS NULL)

-- table variable to store previous timeframe query
declare @invoicegrabprev TABLE (TOTAL_PRICE real, BILLING_ITEM_DESC varchar(MAX))
insert into @invoicegrabprev 
	-- selecting required columns
	select  
		INVOICE_ITEM.TOTAL_PRICE, 
		INVOICE_ITEM.BILLING_ITEM_DESC
	from INVOICE as INVOICE 
		-- joining tables
		right outer join INVOICE_ITEM on INVOICE.INVOICE_NUMBER = INVOICE_ITEM.INVOICE_NUMBER 
		left outer join Z_INVOICE_ITEM_TEST on INVOICE_ITEM.INVOICE_ITEM_NO = Z_INVOICE_ITEM_TEST.INVOICE_ITEM_NO 
		left outer join CUSTOMER 
		inner join SAMPLE on CUSTOMER.NAME = SAMPLE.CUSTOMER on Z_INVOICE_ITEM_TEST.SAMPLE_NUMBER = SAMPLE.SAMPLE_NUMBER 
		left outer join PRODUCT on SAMPLE.PRODUCT = PRODUCT.NAME and SAMPLE.PRODUCT_VERSION = PRODUCT.VERSION 
		left outer join PROJECT on SAMPLE.PROJECT = PROJECT.NAME 
		left outer join CONTRACT_QUOTE on INVOICE.CONTRACT_NUMBER = CONTRACT_QUOTE.CONTRACT_QUOTE_NO 
	where 
	-- user search. Ignore if null
	(@accountnumber IS NULL or CONTRACT_QUOTE.ACCOUNT_NUMBER IN (SELECT * FROM @List))
	-- subtracting timeframe from user entred dates. If user enters same year, subtract 1 year.
	and (INVOICED_ON >=DATEADD(year, case when @fromdate is NULL then 0 when @todate is null then 0 when DATEDIFF(yy, @todate, @fromdate) = 0 then -1 else DATEDIFF(yy, @todate, @fromdate) end, @fromdate) or @fromdate IS NULL)
	and (INVOICED_ON <=DATEADD(year, case when @fromdate is NULL then 0 when @todate is null then 0 when DATEDIFF(yy, @todate, @fromdate) = 0 then -1 else DATEDIFF(yy, @todate, @fromdate) end, @todate) or @todate IS NULL)

-- selecting columns from final pivoted table
select TESTS,  ISNULL(SUM(TOTAL),0) as TOTAL, ISNULL(SUM(PREV_TOTAL),0) as PREV_TOTAL , (ISNULL(SUM(TOTAL),0) - ISNULL(SUM(PREV_TOTAL),0)) as VARIANCE from (
-- selecting columns from joined table
select coalesce(curr.DESCRIPTION, prev.DESCRIPTION) as 'TESTS', curr.TOTAL, prev.PREV_TOTAL from (
	-- selecting grouped columns
	select (
		-- groups stability charges and takes out AR numbers
		case 
			when (CHARINDEX('[', BILLING_ITEM_DESC)>0) then LEFT(BILLING_ITEM_DESC, CHARINDEX('[', BILLING_ITEM_DESC)-1) 
			when BILLING_ITEM_DESC like '%stability%' then 'STABILITY CHARGE'
			else BILLING_ITEM_DESC 
		end) as DESCRIPTION, 
	sum(TOTAL_PRICE) as TOTAL
	from @invoicegrabcurr
	-- grouping
	group by
		(case 
			when (CHARINDEX('[', BILLING_ITEM_DESC)>0) then LEFT(BILLING_ITEM_DESC, CHARINDEX('[', BILLING_ITEM_DESC)-1) 
			when BILLING_ITEM_DESC like '%stability%' then 'STABILITY CHARGE'
			else BILLING_ITEM_DESC
		end)
	)curr 
-- joining previous timeframe table including all entries
full outer join (
-- selecting grouped columns
select (
		-- groups stability charges and takes out AR numbers
		case 
			when (CHARINDEX('[', BILLING_ITEM_DESC)>0) then LEFT(BILLING_ITEM_DESC, CHARINDEX('[', BILLING_ITEM_DESC)-1) 
			when BILLING_ITEM_DESC like '%stability%' then 'STABILITY CHARGE'
			else BILLING_ITEM_DESC 
		end) as DESCRIPTION, 
	sum(TOTAL_PRICE) as PREV_TOTAL
	from @invoicegrabprev
	-- grouping 
	group by
		(case 
			when (CHARINDEX('[', BILLING_ITEM_DESC)>0) then LEFT(BILLING_ITEM_DESC, CHARINDEX('[', BILLING_ITEM_DESC)-1) 
			when BILLING_ITEM_DESC like '%stability%' then 'STABILITY CHARGE'
			else BILLING_ITEM_DESC
		end)
	)prev 
on prev.DESCRIPTION = curr.DESCRIPTION) total_table
-- grouping joined tables
group by TESTS
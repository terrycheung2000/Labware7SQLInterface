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

declare @invoicegrabcurr TABLE (TOTAL_PRICE real, DESCRIPTION varchar(MAX))
insert into @invoicegrabcurr 
select  
INVOICE_ITEM.TOTAL_PRICE, 
PRODUCT.DESCRIPTION
from INVOICE as INVOICE 
right outer join INVOICE_ITEM on INVOICE.INVOICE_NUMBER = INVOICE_ITEM.INVOICE_NUMBER 
left outer join Z_INVOICE_ITEM_TEST on INVOICE_ITEM.INVOICE_ITEM_NO = Z_INVOICE_ITEM_TEST.INVOICE_ITEM_NO 
left outer join CUSTOMER 
inner join SAMPLE on CUSTOMER.NAME = SAMPLE.CUSTOMER on Z_INVOICE_ITEM_TEST.SAMPLE_NUMBER = SAMPLE.SAMPLE_NUMBER 
left outer join PRODUCT on SAMPLE.PRODUCT = PRODUCT.NAME and SAMPLE.PRODUCT_VERSION = PRODUCT.VERSION 
left outer join PROJECT on SAMPLE.PROJECT = PROJECT.NAME 
left outer join CONTRACT_QUOTE on INVOICE.CONTRACT_NUMBER = CONTRACT_QUOTE.CONTRACT_QUOTE_NO 
where 
(@accountnumber IS NULL or CONTRACT_QUOTE.ACCOUNT_NUMBER IN (SELECT * FROM @List))
and (INVOICED_ON >=@fromdate or @fromdate IS NULL)
and (INVOICED_ON <=@todate or @todate IS NULL)

declare @invoicegrabprev TABLE (TOTAL_PRICE real, DESCRIPTION varchar(MAX))
insert into @invoicegrabprev 
select  
INVOICE_ITEM.TOTAL_PRICE, 
PRODUCT.DESCRIPTION
from INVOICE as INVOICE 
right outer join INVOICE_ITEM on INVOICE.INVOICE_NUMBER = INVOICE_ITEM.INVOICE_NUMBER 
left outer join Z_INVOICE_ITEM_TEST on INVOICE_ITEM.INVOICE_ITEM_NO = Z_INVOICE_ITEM_TEST.INVOICE_ITEM_NO 
left outer join CUSTOMER 
inner join SAMPLE on CUSTOMER.NAME = SAMPLE.CUSTOMER on Z_INVOICE_ITEM_TEST.SAMPLE_NUMBER = SAMPLE.SAMPLE_NUMBER 
left outer join PRODUCT on SAMPLE.PRODUCT = PRODUCT.NAME and SAMPLE.PRODUCT_VERSION = PRODUCT.VERSION 
left outer join PROJECT on SAMPLE.PROJECT = PROJECT.NAME 
left outer join CONTRACT_QUOTE on INVOICE.CONTRACT_NUMBER = CONTRACT_QUOTE.CONTRACT_QUOTE_NO 
where 
(@accountnumber IS NULL or CONTRACT_QUOTE.ACCOUNT_NUMBER IN (SELECT * FROM @List))
and (INVOICED_ON >=DATEADD(year, case when @fromdate is NULL then 0 when @todate is null then 0 when DATEDIFF(yy, @todate, @fromdate) = 0 then -1 else DATEDIFF(yy, @todate, @fromdate) end, @fromdate) or @fromdate IS NULL)
and (INVOICED_ON <=DATEADD(year, case when @fromdate is NULL then 0 when @todate is null then 0 when DATEDIFF(yy, @todate, @fromdate) = 0 then -1 else DATEDIFF(yy, @todate, @fromdate) end, @todate) or @todate IS NULL)

select PRODUCTS,  ISNULL(SUM(TOTAL),0) as TOTAL, ISNULL(SUM(PREV_TOTAL),0) as PREV_TOTAL, (ISNULL(SUM(TOTAL),0) - ISNULL(SUM(PREV_TOTAL),0)) as VARIANCE from (
select coalesce(curr.DESCRIPTION, prev.DESCRIPTION) as 'PRODUCTS', curr.TOTAL, prev.PREV_TOTAL from (
	select 
		ISNULL(DESCRIPTION, 'STABILITY CHARGE\CONTRACT DISCOUNT') as DESCRIPTION, 
		sum(TOTAL_PRICE) as TOTAL
	from @invoicegrabcurr
	group by
		ISNULL(DESCRIPTION, 'STABILITY CHARGE\CONTRACT DISCOUNT')
	)curr 
full outer join (
select 
	ISNULL(DESCRIPTION, 'STABILITY CHARGE\CONTRACT DISCOUNT') as DESCRIPTION, 
	sum(TOTAL_PRICE) as PREV_TOTAL
	from @invoicegrabprev
	group by
		ISNULL(DESCRIPTION, 'STABILITY CHARGE\CONTRACT DISCOUNT')
	)prev 
on prev.DESCRIPTION = curr.DESCRIPTION) total_table
group by PRODUCTS
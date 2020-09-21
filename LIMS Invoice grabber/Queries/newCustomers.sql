/*  Created for LABWARE 7 database
	newCustomers.sql

	Inputs: accountnumber, fromdate, todate

	Output: list of customers ordered by first invoiced, last modified
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
			-- sets value into list 
            SET @Value = LTRIM(RTRIM(LEFT(@accountnumber, @Pos - 1)))

            IF @Value <> ''
                INSERT INTO @List (Value) VALUES (@Value) 

            SET @accountnumber = RIGHT(@accountnumber, LEN(@accountnumber) - @Pos)
            SET @Pos = CHARINDEX(',', @accountnumber, 1)
        END
    END  

-- selecting column values
select 
	ACCOUNT_NUMBER,
	account.CUSTOMER, 
	ADDRESS1, 
	ADDRESS2, 
	ADDRESS3, 
	ADDRESS4, 
	ADDRESS5, 
	ADDRESS6, 
	CONTACT, 
	Z_RECEIPT_DIST, 
	MIN(customer.CHANGED_ON) LAST_CHANGED_ON, 
	MIN(invoice.INVOICE_STARTED) FIRST_INVOICE_CREATED, 
	-- adding invoice total as revenue, 0 if null
	case when sum(invoice.ITEM_TOTAL) is null then 0 else sum(invoice.ITEM_TOTAL) end as REVENUE, 
	T_BAD_DEBT 
from account 
-- joining tables
left join customer on account.customer = customer.name
left join invoice on invoice.CUSTOMER = customer.NAME
where 
-- removing demo entries
account.GROUP_NAME != 'DEMO' and 
-- user search
((@accountnumber IS NULL or ACCOUNT_NUMBER IN (SELECT * FROM @List))) and
(@fromdate <= (case when invoice.invoice_started is null then CUSTOMER.changed_on else INVOICE.INVOICE_STARTED end) or @fromdate is null) and
(@todate >= (case when invoice.invoice_started is null then CUSTOMER.changed_on else INVOICE.INVOICE_STARTED end) or @todate is null) 
-- grouping and ordering
group by account.CUSTOMER, ACCOUNT_NUMBER, account.CUSTOMER, ADDRESS1, ADDRESS2, ADDRESS3, ADDRESS4, ADDRESS5, ADDRESS6, CONTACT, Z_RECEIPT_DIST, T_BAD_DEBT
order by (case when MIN(invoice.INVOICE_STARTED) is null then 0 else 1 end) ,FIRST_INVOICE_CREATED desc, LAST_CHANGED_ON desc

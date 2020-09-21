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
	case when sum(invoice.ITEM_TOTAL) is null then 0 else sum(invoice.ITEM_TOTAL) end as REVENUE, 
	T_BAD_DEBT 
from account 
left join customer on account.customer = customer.name
left join invoice on invoice.CUSTOMER = customer.NAME
where 
account.GROUP_NAME != 'DEMO' and 
((@accountnumber IS NULL or ACCOUNT_NUMBER IN (SELECT * FROM @List))) and
(@fromdate <= (case when invoice.invoice_started is null then CUSTOMER.changed_on else INVOICE.INVOICE_STARTED end) or @fromdate is null) and
(@todate >= (case when invoice.invoice_started is null then CUSTOMER.changed_on else INVOICE.INVOICE_STARTED end) or @todate is null) 
group by account.CUSTOMER, ACCOUNT_NUMBER, account.CUSTOMER, ADDRESS1, ADDRESS2, ADDRESS3, ADDRESS4, ADDRESS5, ADDRESS6, CONTACT, Z_RECEIPT_DIST, T_BAD_DEBT
order by (case when MIN(invoice.INVOICE_STARTED) is null then 0 else 1 end) ,FIRST_INVOICE_CREATED desc, LAST_CHANGED_ON desc

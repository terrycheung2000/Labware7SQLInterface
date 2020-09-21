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



SELECT DISTINCT 
						 t.TEST_NUMBER,
						 t.ANALYSIS, 
						 t.REPORTED_NAME,
                         s.SAMPLE_NUMBER, 
						 s.PROJECT, 
						 s.CUSTOMER, 
						 s.Z_CUST_PO_NO AS SAMPLE_PO_NUMBER, 
						 p.T_PO_NUMBER AS PROJECT_PO_NO, 
						 p.T_CONTRACT_NUMBER AS PROJECT_CONTRACT_NO, 
						 cq.CONTRACT_QUOTE_NO, 
                         a.ACCOUNT_NUMBER,
						 s.STATUS,
						 s.DATE_STARTED,
						 s.DATE_COMPLETED,
						 s.DATE_REVIEWED,
						 s.CHANGED_ON, 
						 s.DUE_DATE,
						 CASE WHEN i.INVOICE_NUMBER IS NULL THEN 0 ELSE i.INVOICE_NUMBER END AS existing_invoice,
						 cqi.CATALOGUE_ITEM_NO,
						 cqi.COST_ITEM_NO
FROM SAMPLE AS s 
					LEFT OUTER JOIN PROJECT AS p ON s.PROJECT = p.NAME 
					LEFT OUTER JOIN CUSTOMER AS c ON s.CUSTOMER = c.NAME 
					LEFT OUTER JOIN ACCOUNT AS a ON a.CUSTOMER = c.NAME AND a.REMOVED = 'F' 
					LEFT OUTER JOIN CONTRACT_QUOTE AS cq ON a.ACCOUNT_NUMBER = cq.ACCOUNT_NUMBER AND cq.REMOVED = 'F' 
					INNER JOIN TEST AS t ON t.SAMPLE_NUMBER = s.SAMPLE_NUMBER AND t.INVOICE_NUMBER = 0 AND t.STATUS = 'A' 
					LEFT OUTER JOIN INVOICE AS i ON i.PO_NUMBER = s.Z_CUST_PO_NO AND i.CLOSED = 'F'
					left join CONTRACT_QUOTE_ITM as cqi on cq.CONTRACT_QUOTE_NO= cqi.CONTRACT_QUOTE_NO and t.ANALYSIS = cqi.ITEM_CODE
WHERE        (s.APPROVED = 'T') AND (s.STATUS NOT IN ('X', 'R', 'C'))
			 and (@accountnumber IS NULL or cq.ACCOUNT_NUMBER in (SELECT * FROM @List))
			 and ((s.SAMPLE_NUMBER = @samplenum or @samplenum IS NULL) or (s.Z_PRODUCT_LOT = @lot or @lot IS NULL))
			 and (s.DUE_DATE >= @fromdate or @fromdate IS NULL)
			 and (s.DUE_DATE <= @todate or @todate IS NULL)



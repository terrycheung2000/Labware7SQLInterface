   M S B A M L   `   `   `  ���� O  KLIMS Invoice grabber, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:LIMS_Invoice_grabberLIMS_Invoice_grabber  0    *LIMS_Invoice_grabber.InvoiceItemDetailView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     4local"clr-namespace:LIMS_Invoice_grabber       � Title$
  Info��$��493.954��$��589.207�� � Icon$ CAL-logo-main-ST.png�� � 
ResizeMode$ NoResize=�.��� ���	#FFE6E6E6.+��� $��Company:��$
��Left=�$��	70,10,0,0q�$	��Top=��� $��Account Number:��$
��Left=�$��
293,10,0,0q�$	��Top=��� $��Invoice Number:��$
��Left=�$��
36,124,0,0q�$	��Top=� J� RenderTransformOrigin$ 
0.402,0.56&��� $��Product Description:��$
��Left=�$��
15,243,0,0q�$	��Top=��� $��Sample Number:��$
��Left=�$��
34,299,0,0q�$	��Top=��� $��Z__Product__Lot:��$
��Left=�$��303,271,0,0q�$	��Top=��� $��Invoice Total:��$
��Left=�$	��Top=�$��312,124,0,0q��� $��Billing Item Desc:��$
��Left=�$��
31,355,0,0q�$	��Top=��� $��Invoiced on:��$
��Left=�$��
58,152,0,0q�$	��Top=��� $
��Left=�$��20��$��
21,216,0,0q�$	��Top=�$	��525���� $��Unit Price:��$
��Left=�$��
68,383,0,0q�$	��Top=��� $��Total Price:��$
��Left=�$��
65,411,0,0q�$	��Top=��� $��	Quantity:��$
��Left=�$��333,383,0,0q�$	��Top=��� $��Contact:��$
��Left=�$��	79,39,0,0q�$	��Top=��� $��
PO Number:��$
��Left=�$��
58,271,0,0q�$	��Top=��� $��Phone Number:��$
��Left=�$��
298,39,0,0q�$	��Top=��� $��
Recd Date:��$
��Left=�$��325,327,0,0q�$	��Top=��� $��Invoice Item No :��$
��Left=�$��
32,180,0,0q�$	��Top=��� $��Email:��$
��Left=�$��	92,67,0,0q�$	��Top=�$ 0.512,1.084&��� $��Cost Item No:��$
��Left=�$��
49,327,0,0q�$	��Top=�$ 0.512,1.084&��� $��Rush:��$
��Left=�$��353,180,0,0q�$	��Top=�$ 0.512,1.084&��� -    ��Name$ companyName��$
��Left=�$��23��$��
138,13,0,0q� �� TextWrapping$
 Wrap=�$	��Top=�$	��145�� �� 
IsReadOnly . �� -   $ contact��$
��Left=�$��23��$��
138,42,0,0q�$
 Wrap=�$	��Top=�$	��145�� . �� -   $ zReceiptDist��$
��Left=�$��49��$��
138,70,0,0q�$
 Wrap=�$	��Top=�$	��420�� . �� -   $ invoiceNumberBox��$
��Left=�$��23��$��138,127,0,0q�$
 Wrap=�$	��Top=�$	��145�� . �� -   $ 
invoicedOn��$
��Left=�$��23��$��138,155,0,0q�$
 Wrap=�$	��Top=�$	��145�� . �� -   $ accountNumber��$
��Left=�$��23��$��
396,13,0,0q�$
 Wrap=�$	��Top=�$	��162�� . �� -   $ phoneNum��$
��Left=�$��23��$��
396,42,0,0q�$
 Wrap=�$	��Top=�$	��162�� . $ 0.494,0.731&��� -   $ 	itemTotal��$
��Left=�$��23��$��396,127,0,0q�$
 Wrap=�$	��Top=�$	��162�� . �� -	   $ description��$
��Left=�$��23��$��138,246,0,0q�$
 Wrap=�$	��Top=�$	��420�� . �� -
   $ 	tPoNumber��$
��Left=�$��23��$��138,274,0,0q�$
 Wrap=�$	��Top=�$	��145�� . �� -   $ sampleNumber��$
��Left=�$��23��$��138,302,0,0q�$
 Wrap=�$	��Top=�$	��145�� . �� -   $ zProductLot��$
��Left=�$��23��$��396,274,0,0q�$
 Wrap=�$	��Top=�$	��162�� . �� -   $ billingItemDesc��$
��Left=�$��23��$��138,358,0,0q�$
 Wrap=�$	��Top=�$	��420�� . �� -   $ 	unitPrice��$
��Left=�$��23��$��138,386,0,0q�$
 Wrap=�$	��Top=�$	��145�� . �� -   $ quantity��$
��Left=�$��23��$��396,386,0,0q�$
 Wrap=�$	��Top=�$	��162�� . �� -   $ 
totalPrice��$
��Left=�$��23��$��138,414,0,0q�$
 Wrap=�$	��Top=�$	��145�� . �� -   $ recdDate��$
��Left=�$��23��$��396,330,0,0q�$
 Wrap=�$	��Top=�$	��162�� . �� -   $ invoiceItemNo��$
��Left=�$��23��$��138,183,0,0q�$
 Wrap=�$	��Top=�$	��145�� . �� -   $ 
costItemNo��$
��Left=�$��23��$��138,330,0,0q�$
 Wrap=�$	��Top=�$	��145�� . �� -   $ zRush��$
��Left=�$��23��$��396,183,0,0q�$
 Wrap=�$	��Top=�$	��162�� . �� $��Login Date:��$
��Left=�$��321,299,0,0q�$	��Top=��� -   $ 	loginDate��$
��Left=�$��23��$��396,302,0,0q�$
 Wrap=�$	��Top=�$	��162�� . 
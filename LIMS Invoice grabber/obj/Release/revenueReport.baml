   M S B A M L   `   `   `  ���� O  KLIMS Invoice grabber, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:LIMS_Invoice_grabberLIMS_Invoice_grabber  (    "LIMS_Invoice_grabber.revenueReport   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     4local"clr-namespace:LIMS_Invoice_grabber     -     �Name$  
reportPage�� � Title$ Revenue Report��$��759.219��$��889.032�� � Icon$ CAL-logo-main-ST.png��.��� .+��� $��Company:��$
��Left=�$��	10,10,0,0q�$	��Top=��� $��Contact:��$
��Left=�$��	10,38,0,0q�$	��Top=��� $��Phone Number:��$
��Left=�$��
253,39,0,0q�$	��Top=��� $��Email:��$
��Left=�$��
492,38,0,0q�$	��Top=��� -    ��Name$ contactNameBox��$
��Left=�$��23��$��	69,40,0,0q� �� TextWrapping$
 Wrap=�$	��Top=�$	��181�� �� 
IsReadOnly . �� -   $ phoneNumberBox��$
��Left=�$��23��$��
348,40,0,0q�$
 Wrap=�$	��Top=�$	��139�� . �� -   $ emailbox��$��23��$��538,40,15,0q�$
 Wrap=�$	��Top=� . �� -   $ companyNameBox��$
��Left=�$��23��$��	73,13,0,0q�$
 Wrap=�$	��Top=�$	��262�� . �� $��Total Revenue:��$
��Left=�$��
340,10,0,0q�$	��Top=��� -   $ grandTotalBox��$
��Left=�$��23��$��
431,13,0,0q�$
 Wrap=�$	��Top=�$	��117�� . � $��10,75,17,20q� � ColumnDefinitions �� $
��312* ��� $��5 ��� $
��535* �.+�&   System.Windows.Controls.DataGrid  -     Name$ 	testsGrid��   
IsReadOnly . 	   CanUserSortColumns	 .  �� $��Stretch=�$	��635��$��1��$��5��  -   $ productsGrid�� . 	 .  $��2���� -	   
 ��Name$
 toExel��$��Send to Exel��$��	0,13,17,0q�$	��Top=�$��Right=�$��97���� $��Total Previous:��$
��Left=�$��
553,10,0,0q�$	��Top=��� -
   $ grandTotalPrevBox��$
��Left=�$��23��$��
639,13,0,0q�$
 Wrap=�$	��Top=�$	��113��
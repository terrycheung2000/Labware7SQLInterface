   M S B A M L   `   `   `  ���� O  KLIMS Invoice grabber, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:LIMS_Invoice_grabberLIMS_Invoice_grabber  )    #LIMS_Invoice_grabber.invoiceGrabber   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     4local"clr-namespace:LIMS_Invoice_grabber       � Title$  LIMS Invoice Grabber��$	��890��$	��520��$��576.4��$	��961�� � Icon$ CAL-logo-main-ST.png��.���  � ColumnDefinitions ��  � RowDefinitions �� $��75 ��� .+��� -    ��Name$ customer��$��	10,47,0,0q� �� TextWrapping$
 Wrap=�$��23��$	��Top=�$
��Left=�$	��214���� $!��Company Name/Account Number��$
��Left=�$	��Top=�$	��217��$��7,21,0,0q�$��26��(  "System.Windows.Controls.DatePicker  -     Name$
 from��$��
515,46,0,0q�$
��Left=�$	��141��$��24��$	��Top=��� -    ��Name$ search��$��Search��$��
844,47,0,0q�$��24��$	��Top=�$
��Left=�$��81�� �� 	IsDefault .   -   $ to��$��
685,46,0,0q�$
��Left=�$	��141��$��24��$	��Top=��� $
��From��$
��Left=�$��
475,45,0,0q�$	��Top=�$��26��$��37���� $��to��$
��Left=�$��
659,46,0,0q�$	��Top=�$��26��$��21���� -   	 ��Name$
	 tabs��$��
10,0,22,10q�$��1��$	��650��$	��355��.@��� $��Invoice Items��$��0��.��� ���	#FFE5E5E5.+�&   System.Windows.Controls.DataGrid  -   
  Name$
 queryResult��$	��450��$	��340��   
IsReadOnly . $
��3840��$
��2160��   ClipboardCopyMode$ IncludeHeader=�$��0,26,0,0q� �� 
IsSelected .  � ContextMenu �� .@�m� -    m�Name$ invoiceItemDetail��$��Detailed View��m� -   $ sampleSubmission��$��Sample Submission Form��	� -	    	�Name$ sortInvoice��$��Invoice��$��	0,6,102,0q�$	��Top=� d� 	IsChecked$
 TrueY�$��Right=�$��56���� $��	Order by:��$	��Top=�$��
0,-1,163,0q�$��Right=�$��59��	� -
   $ sortItem��$��Invoice Item��$��0,6,9,0q�$	��Top=�$��Right=�$��84���� -   $ revenueGenBtn��$��Generate Revenue Report��$
��Left=�$��	108,3,0,0q�$	��Top=�$	��155��$y�Hidden=��� -   $��Send to Excel��$
��Left=�$��3,3,0,0q�$	��Top=�$	��100���� $��Invoices��$��1��.��� ���	#FFE5E5E5.+�  -   $
 invoiceGrid��$��0,0,0,40q�$	��450��$	��340�� . $
��3840��$
��2160��$ IncludeHeader=�   SelectionMode$ Extended=� �� .@�m� -   $ 
getInvoice��$��Save to Zip��m� -   $ getInvoicePDF��$��Save to PDF���� -   $
 save��$��Save All��$��
0,0,115,10q�$��20��$��Bottom=�$��Right=�$��76�� J� RenderTransformOrigin$ 0.921,0.478&��� -   $��Send to Excel��$��	0,0,10,10q�$��20��$��Bottom=�$��Right=�$	��100���� $	��COA��$��2��.��� ���	#FFE5E5E5.+�  -   $
 coaGrid��$-��'0,0,0,40                               q�$	��450��$	��340�� . $
��3840��$
��2160��$ IncludeHeader=�$ Extended=� �� .@�m� -   $ getCOA��$��Save to Zip��m� -   $ 	getCOAPDF��$��Save to PDF���� -   $ saveCOA��$��Save All��$��
0,0,115,10q�$��20��$��Bottom=�$��Right=�$��76���� -   $��Send to Excel��$��	0,0,10,10q�$��20��$��Bottom=�$��Right=�$	��100���� $��Missing Invoice��$��3��.��� ���	#FFE5E5E5.+�  -   $
 missingGrid�� . $
��3840��$
��2160��$ IncludeHeader=�$ Extended=�$-��'0,0,0,40                               q�   RowStyle ��  �� 
TargetType)  #System.Windows.Controls.DataGridRow"   �� Triggers ��  �� Binding �� *COST_ITEM_NO+ �� Value Z� .a���  �� Property � �� �� Value
 �\\���� -   $ printButton��$��Send to Excel��$��	0,0,10,10q�$��20��$��Bottom=�$��Right=�$	��100���� -    ��Name$ inProgressTests��$��Tests In Progress��$��4��.��� ���	#FFE5E5E5.+�  -   $
 testGrid�� . $
��3840��$
��2160��$ IncludeHeader=�$ Extended=�$��0,0,0,40q��� -   $��Send to Excel��$��	0,0,10,10q�$��20��$��Bottom=�$��Right=�$	��100���� $��Invoice Number��$
��Left=�$��
233,21,0,0q�$	��Top=�$��26��$��94���� -   $ invoiceNumber��$
��Left=�$��23��$��
237,47,0,0q�$
 Wrap=�$	��Top=�$	��120���� $��Sample/Lot Number��$
��Left=�$��
366,21,0,0q�$	��Top=��� -   $ arlot��$
��Left=�$��23��$��
370,47,0,0q�$
 Wrap=�$	��Top=�$��99��o� $
��Left=�$��21��$	��Top=�$	��953��.@�m� $��Administration Tools��$��21��$	��125��.@�m� -   $ functionAdd��$��Add SQL Functions��m� -   $ changeConnection��$��Change Connection��
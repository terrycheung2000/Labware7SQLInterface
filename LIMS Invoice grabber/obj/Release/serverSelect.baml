   M S B A M L   `   `   `  ���� O  KLIMS Invoice grabber, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:LIMS_Invoice_grabberLIMS_Invoice_grabber  '    !LIMS_Invoice_grabber.serverSelect   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     4local"clr-namespace:LIMS_Invoice_grabber       � Title$  Connect to SQL Server��$��176.594��$	��355�� � 
ResizeMode$ NoResize=� � ShowInTaskbar .   � Icon$ CAL-logo-main-ST.png��.��� ���	#FFE8E8E8.+��� $��Server Name:��$
��Left=�$��	10,10,0,0q�$	��Top=��� $��Login:��$
��Left=�$��	35,41,0,0q�$	��Top=�$��56���� -    ��Name$ serverNameBox��$
��Left=�$��
150,10,0,0q�$	��Top=�$	��179���� -    ��Name$ usernameBox��$
��Left=�$��23��$��
181,41,0,0q� �� TextWrapping$
 Wrap=�$��cal_user��$	��Top=�$	��148��G� -    G�Name$ passwordBox��$
��Left=�$��
181,70,0,0q�$	��Top=�$	��148��$��23�� G� Password$ calabs���� $��	Password:��$
��Left=�$��	35,67,0,0q�$	��Top=��� -   	 ��Name$	 	cancelBtn��$��Cancel��$
��Left=�$��199,109,0,0q�$	��Top=�$��75��
 �� IsCancel
 . �� -   $	 	dbConnect��$��Connect��$
��Left=�$��
65,109,0,0q�$	��Top=�$��75��
   M S B A M L   `   `   `  ����E  AIPC_Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:IPC_Client
IPC_Client  5   	       IPC_Client.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:IPC_Client     5   	   -   5          � Icon$  messenger.ico��5       � 
ResizeMode$ CanMinimize=�5
       � Title$ TAAK Messenger��5   	   $	��450��6    $	��800��6-   .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $	��300 �6   �� 5      $
��auto �6    � ColumnDefinitions 5   
   �� 5      $
��auto �6   �� 5      $	��200 �6   �� 5      $	��100 �6   �� 5      $
��310* �6   �� 5      $
��102* �6   �� 5      $��6* �6   �� 5      $��5* �6   .+�5   
   �� $��3��6   $��0��6   $��Message:��6-   $��2��6@   $��0,0.6,199.4,-0.2q�6T   �� 5      -   5         ��Name$ nameBox��5      $��0��6   $��0��6*   $��2��6:   $��Enter a username��6N   6f   �� 5   
   -   5        $ 
connectBox��5      $��0��6&   $��0��63   $��2��6C   $��Enter an IP��6W   6j   �� 5    
   -   5        $��	TextBoxIn��5    Z   $��3��6   $��1��6   $��3��6/   $��4,3.6,1.4,1.8q�6C   6k   �� 5"   
   -   5        $��
TextScreen��5"   �   $��3q�6   
���   �
�������$��2��6M   $��5��6Z   $��0,10,1.4,26.2q�6n    �� 
IsReadOnly . $��2��6�   " �� VerticalScrollBarVisibility$
 Auto=�6�    �� TextWrapping$
 Wrap=�6�   $	��300��6�   �� 5$   
   -   5        	 ��Name$	 userName��5$      $��0��6#   $��2��60   $
��Save��6@   6O   �� 5%   
   -   5        $	 connectToServer��5%      $��2��6*   $��Connect��6:   6L   �� 5&   
   -   5        $��TextSendBtn��5&   .   $��3��6   $��4��6   $
��Send��6A   6P   �� 5'   
   -	   5        
 ��Name$
 nameAccepted��5'      $��3��6*   $
��Left=�6:   $
��Wrap=�6U   ���	#FFF91E03$��
Username: ��6�   �� 5(   
   -
   5        $
 connectionAccepted��5(      $��3��60   $
��Left=�6@   $��1��6\   $
��Wrap=�6i   ���	#FFF91E03$��Status: ��6�   5*      5+      
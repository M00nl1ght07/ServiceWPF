   M S B A M L   `   `   `  ����E  AServiceWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:ServiceWPF
ServiceWPF  5   	       ServiceWPF.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:ServiceWPF     5   	     � Title$  Service Desk��5   	   $	��800��6   $
��1200��6+    � WindowStartupLocation$ CenterScreen=�5	   	   
������� � WindowStyle$
 None=�5   	    � AllowsTransparency .  � 
ResizeMode$ 	CanResize=�5   	   .��5      �� 
�������$��1q�6    ���#EEEEEE._�5   
   �  � RowDefinitions 5      �� 5      $��60 �6    �� 5      .+�5      � -   5        ���#2196F3 � ColumnDefinitions 5      �� 5      $	��250 �6'   �� 5      �� 5      $
��Auto �6'   �� 5      $
��Auto �6'   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��20,0,0,0q�5       $��Center=�5!      .+�5"      �� $��/images/logo.png��6   $��40��5#      $��40��5$      �� 5%      $��Service Desk��6    
�������$��22(�5'      $��Medium"�5(      $��15,0,0,0q�5)      $��Center=�5*      5+      �� 5.      -   5         ��Name$ CurrentPageTitle��5/      $��1��5.      $��Мои заявки��50      
�������$��20(�52      $��Center=�53      $
��20,0q�54      �� 57      $��2��6   $ 
Horizontal=�58      $
��20,0q�59      .+�5:      �� -   5        $ UserNameText��5:       $��Иван Иванов��5;      
�������$��16(�5=      $��Center=�5>      $��0,0,20,0q�5?      �� 5@      -   5        $��
Выход��5@      $	��100��5A      $��35��5B         ExitButtonStyle#��[  5C      5D      5E      �� 5H      -   5        $��3��5H        CloseButtonStyle#��[ 5I      $
��20,0q�5J      5K      5L      � 5O      $��1��6    5P      �� 5Q      $	��250 �6'   �� 5R      .+�5V      �� ���#F5F5F5$��0,0,1,0q�5W      ���#EEEEEE._�5Y      �� -   5        	 ��Name$	 	MenuPanel��5Y   !   $
��0,20q�64   5[      5\      �� 5_      $��1��6   
�������$��20q�5a      ._�5b      � .+�5d       � -   5        
  �Name$
 	MainFrame��5d         � NavigationUIVisibility$ Hidden=�5e       � 5h      -   5         �Name$ NotificationGrid��5h       K� ZIndex$
 9999��5i      $��Center=�5j      $	��Top=�5k      $��0,20,0,0q�5l      5m      5n      5o      5p      5q      5r      
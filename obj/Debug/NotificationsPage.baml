   M S B A M L   `   `   `  ����E  AServiceWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:ServiceWPF
ServiceWPF  5      "    ServiceWPF.NotificationsPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      *localclr-namespace:ServiceWPF     5        N� Title$  Уведомления��5       N� 
Background
 �����.��5      � $��0q�6    � RowDefinitions 5	   
   �� 5
      $
��Auto �6   �� 5      .+�5   
   � $��0,0,0,20q�6    � ColumnDefinitions 5      �� 5      �� 5      $
��Auto �6#   .+�5      �� $+��%История уведомлений��6   $��24(�5      $��Medium"�5      �� 5      -   5        $��1��5      $;��5Отметить все как прочитанные��5      $��35��5      $
��15,0q�5      5      5      �� 5!   
   -   5         ��Name$ NotificationsList��5!      $��1��5"      $��0q�5#      
������ $��Stretch=�5%      ��5&      �� 5'      .R�5(      ��  �� Padding$ 15q�6   $��0,0,0,1q�5*      ���#E0E0E0 �� CornerRadius$ 4��5,      $��Stretch=�5-      ��5)      �� *IsRead+ #  ReadStatusToBackgroundConverter �� 	Converter# [  ��5.      �� *ActualWidth+ �� RelativeSource �� 	 �� AncestorType$	 ListViewL�._�5/      �  50      �� 51   "   $
��Auto �60   �� 52   "   $
��Auto �60   �� 53   "   $
��Auto �60   .+�56      �� $��Medium"�57   &   $��16(�58   &   ��56   (   �� *Title+58   &   �� 5:      $��1��6(   $	��0,5q�5<   &   $
��Wrap=�5=   &   ��5;   &   �� *	Message+5=   &   �� 5?      $��2��6(   ���#757575$��12(�5B   &   ��5@   &   �� *CreatedDate+5B   &   5C      5D      5E      5G      5H      5I      
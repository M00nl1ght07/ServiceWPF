   M S B A M L   `   `   `  ����  
ServiceWPF     ServiceWPF.AllRequestsPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        N� Title$  Все заявки��5       N� 
Background
 �����.��5      � $��0q�6    � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      .+�5   
   � $��0,0,0,20q�6    � ColumnDefinitions 5      �� 5      �� 5      $
��Auto �6#   �� 5      $
��Auto �6#   .+�5      �� 
�������$��1q�5      ���#E0E0E0 �� CornerRadius$ 4��5      $��0,0,10,0q�5      ._�5      �  5      �� 5      $
��Auto �6+   �� 5      .+�5       �� $
��🔍��6    $
��10,0q�5!      $��Center=�5"      ���#7575755#      �� 5$      -   5         ��Name$ 	SearchBox��5*      $��1��5$      $��0q�5%      $	��0,8q�5&      
������ $��0,0,10,0q�5(      $��Center=�5)      5+      5,      5-      �� 50      -   5         ��Name$ StatusFilter��52      $��1��50      $	��150��51      
�������$��0,0,10,0q�54       �� SelectedIndex$ 0��55      .@�57      �� $��Все статусы��6   �� 58      $��
Новая��6   �� 59      $��В работе��6   �� 5:      $��На проверке��6   �� 5;      $��Завершена��6   �� 5<      $��Отменена��6   5=      �� 5?      -   5        $ ExecutorFilter��5A      $��2��5?      $	��150��5@      
�������$��0,0,10,0q�5C      5F      5G      �� 5J   
   $��1��6   $��1q�5K      ���#E0E0E0
�������$ 4��5N      ._�5O      �� $
��Auto=�6   $��Disabled=�5P      $��0,0,8,0q�5Q      .��5R      �� -   5         ��Name$ RequestsList��5R      $��0q�5S      
�������$��Disabled=�5U      $��Disabled=�5V      ��5X      �� 5Y      .R�5Z      �� 5[      ��5^      �� 5_      	 �� 
TargetType"	 ��6    .�5`      �� 
 �� Property
 � �� �� Value$ Stretch=�6K   �� 5a      
 � ��$ Stretch=�6D   �� 5b      
 � ��$ 0q�68   �� 5c      
 � ��
 �����6;   �� 5d      
 � ��$ 0q�6@    �� Triggers 5e      P� 5f   "    P� Property �@J�IsMouseOver P� Value . . �5g   &   �� 
 � �� �#F5F5F56C   5h   #   5j      ��5m      �� 5n      .R�5o      ��  �� Padding$ 15q�6%   
������ $��0,0,0,1q�5q   %   ���#E0E0E0._�5s   "   �  5t   &   �� 5u   *   �� 5v   *   $
��Auto �6;   .+�5z   &   �� .+�5{   *   �� $��16(�5|   2   $��Medium"�5}   2   ��5{   4   �� *Title+5}   2   �� 5~   *   $	��0,5q�5   2   $
��Wrap=�5�   2   ���#757575��5~   4   �� *Description+5�   2   �� 5�   *    �� Orientation$ 
Horizontal=�65   $��0,5,0,0q�5�   3   .+�5�   .   �� ���#757575��68   �� *CreatedDate+5�   6   �� 5�   .   $�� • ��68   ���#7575755�   6   �� 5�   .   ���#2196F3��68   �� *Status+5�   6   �� 5�   .   $�� • ��68   ���#7575755�   6   �� 5�   .   ���#757575��68   �� *
Executor+5�   6   5�   +   5�   '   �� 5�   &   $��1��61   $ 
Horizontal=�5�   /   $��20,0,0,0q�5�   /   .+�5�   *   �� -   5        $��Отменить��5�   1   $��35��5�   1   $	��100��5�   1   
 � Tag 5�   1   �� 5�   '   5�   #   5�      5�      5�      5�      5�      5�      5�      
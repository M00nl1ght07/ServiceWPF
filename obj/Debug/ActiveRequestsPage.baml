   M S B A M L   `   `   `  ����  
ServiceWPF#    ServiceWPF.ActiveRequestsPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        N� Title$)  #Мои заявки в работе��5       N� 
Background
 �����.��5      � $��0q�6    � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      .+�5   
   � $��0,0,0,20q�6    � ColumnDefinitions 5      �� 5      �� 5      $
��Auto �6#   .+�5      �� 
�������$��1q�5      ���#E0E0E0 �� CornerRadius$ 4��5      $��0,0,10,0q�5      ._�5      �  5      �� 5      $
��Auto �6+   �� 5      .+�5      �� $
��🔍��6    $
��10,0q�5       $��Center=�5!      ���#7575755"      �� 5#      -   5         ��Name$ 	SearchBox��5)      $��1��5#      $��0q�5$      $	��0,8q�5%      
������ $��0,0,10,0q�5'      $��Center=�5(      5*      5+      5,      �� 5/      -   5         ��Name$ StatusFilter��51      $��1��5/      $	��150��50      
�������.@�54      �� $��Все статусы��6   �� 55      $��В работе��6   �� 56      $��На проверке��6   �� 57      $��Завершена��6   58      59      �� 5<   
   $��1��6   $��1q�5=      ���#E0E0E0
�������$ 4��5@      ._�5A      �� $
��Auto=�6   $��Disabled=�5B      $��0,0,8,0q�5C      .��5D      �� -   5         ��Name$ RequestsList��5D      $��0q�5E      
�������$��Disabled=�5G      $��Disabled=�5H      ��5J      �� 5K      .R�5L      �� 5M      ��5P      �� 5Q       �� 
TargetType" ��6    .�5R      �� 	 �� Property	 � ��
 �� Value$
 Stretch=�6K   �� 5S      	 � ��$
 Stretch=�6D   �� 5T      	 � ��$
 0q�68   �� 5U      	 � ��

 �����6;   �� 5V      	 � ��$
 0q�6@    �� Triggers 5W      P� 5X   "    P� Property �@J�IsMouseOver P� Value . . �5Y   &   �� 	 � ��
 �#F5F5F56C   5Z   #   5\      ��5_      �� 5`      .R�5a      ��  �� Padding$ 15q�6%   
������ $��0,0,0,1q�5c   %   ���#E0E0E0._�5e   "   �  5f   &   �� 5g   *   �� 5h   *   $
��Auto �6;   .+�5l   &   �� .+�5m   *   �� $��16(�5n   2   $��Medium"�5o   2   ��5m   4   �� *Title+5o   2   �� 5p   *   $	��0,5q�5q   2   $
��Wrap=�5r   2   ���#757575��5p   4   �� *Description+5s   2   �� 5t   *    �� Orientation$ 
Horizontal=�65   $��0,5,0,0q�5u   3   .+�5v   .   �� ���#757575��68   �� *CreatedDate+5w   6   �� 5x   .   $�� • ��68   ���#7575755y   6   �� 5z   .   ���#2196F3��68   �� *Status+5{   6   5|   +   5}   '   �� 5�   &   $��1��61   $ 
Horizontal=�5�   /   $��20,0,0,0q�5�   /   .+�5�   *   �� -   5        $	��150��5�   3   $��35��5�   2   $��0,0,10,0q�5�   2   
 � Tag 5�   2   �� *	RequestID+.@�5�   .   �� $��В работе��6;   �� 5�   .   $��На проверке��6;   �� 5�   .   $��Завершена��6;   5�   +   5�   '   5�   #   5�      5�      5�      5�      5�      5�      5�      
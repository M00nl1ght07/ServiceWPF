   M S B A M L   `   `   `  ����   
ServiceWPF     ServiceWPF.AllRequestsPage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       N� Title$  Все заявки�� N� 
Background
 �����.��� $��0q� � RowDefinitions �� $
��Auto ��� .+�� $��0,0,0,20q� � ColumnDefinitions �� �� $
��Auto ��� $
��Auto �.+��� 
�������$��1q����#E0E0E0 �� CornerRadius$ 4��$��0,0,10,0q�._��  �� $
��Auto ��� .+��� $
��🔍��$
��10,0q�$��Center=����#757575�� -    ��Name$ 	SearchBox��$��1��$��0q�$	��0,8q�
������ $��0,0,10,0q�$��Center=��� -    ��Name$ StatusFilter��$��1��$	��150��
�������$��0,0,10,0q� �� SelectedIndex$ 0��.@��� $��Все статусы���� $��
Новая���� $��В работе���� $��На проверке���� $��Завершена���� $��Отменена���� -   $ ExecutorFilter��$��2��$	��150��
�������$��0,0,10,0q��� $��1��$��1q����#E0E0E0
�������$ 4��._��� $
��Auto=�$��Disabled=�$��0,0,8,0q�.���� -    ��Name$ RequestsList��$��0q�
�������$��Disabled=�$��Disabled=����� .R��� ���� 	 �� 
TargetType"	 ��.��� 
 �� Property
 � �� �� Value$ Stretch=��� 
 � ��$ Stretch=��� 
 � ��$ 0q��� 
 � ��
 ������� 
 � ��$ 0q� �� Triggers P�  P� Property �@J�IsMouseOver P� Value . . ��� 
 � �� �#F5F5F5���� .R���  �� Padding$ 15q�
������ $��0,0,0,1q����#E0E0E0._��  �� �� $
��Auto �.+��� .+��� $��16(�$��Medium"����� *Title+�� $	��0,5q�$
��Wrap=����#757575���� *Description+��  �� Orientation$ 
Horizontal=�$��0,5,0,0q�.+��� ���#757575���� *CreatedDate+�� $�� • �����#757575�� ���#2196F3���� *Status+�� $�� • �����#757575�� ���#757575���� *
Executor+�� $��1��$ 
Horizontal=�$��20,0,0,0q�.+��� -   $��Отменить��$��35��$	��100��
 � Tag �� 
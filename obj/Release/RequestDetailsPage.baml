   M S B A M L   `   `   `  ����   
ServiceWPF#    ServiceWPF.RequestDetailsPage   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       N� Title$  Детали заявки�� N� 
Background
 �����.��� $��0q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� .+��� -   $��← Назад��$��35��$
��15,0q�$
��Left=�$��0,0,0,20q�
������ ���#2196F3$��0q� � Cursor$
 Hand���� $��1��.+�� $��0,0,0,20q� � ColumnDefinitions �� �� $
��Auto �.+��� .+��� -    ��Name$ TitleTextBlock��$*��$Не работает принтер��$��24(�$��Medium"���  �� Orientation$ 
Horizontal=�$��0,10,0,0q�.+��� $��Создана: �����#757575�� -   $ CreatedDateTextBlock��$��
15.03.2024�����#757575�� $�� • �����#757575$	��5,0q��� -   $ StatusTextBlock��$��В работе�����#2196F3�� $�� • �����#757575$	��5,0q��� $��Исполнитель: �����#757575�� -   $ ExecutorTextBlock��$��Не назначен�����#2196F3�� $��1�����#FFF3E0 �� Padding$
 12,6q� �� CornerRadius$ 4��._��� -   $ PriorityTextBlock��$��Высокий�����#F57C00�� $��Описание��$��16(�$��Medium"�$��0,0,0,10q��� -   $ DescriptionTextBlock��$����Принтер HP в кабинете 405 не печатает документы. При отправке на печать появляется ошибка.��$
��Wrap=����#757575$��0,0,0,30q��� $ 
Horizontal=�$��0,5,0,0q�.+��� $��Создал: ���� -   $ CreatedByTextBlock��$�� ���� $ 
Horizontal=�$��0,5,0,0q�.+��� $%��Дата завершения: ���� -	   $ CompletionDateTextBlock��$�� ���� $ 
Horizontal=�$��0,5,0,0q�.+��� $-��'Последнее изменение: ���� -
   $ LastModifiedDateTextBlock��$�� ���� -   	 ��Name$	 LeaveReviewButton��$!��Оставить отзыв��$��35��$	��150��$
��Left=�$��	0,10,0,20q�$y�	Collapsed=�� $��2�� �� $	��300 ��� .+�]� $��0,0,20,0q�.+��� $'��!История изменений��$��16(�$��Medium"�$��0,0,0,15q�$	��Top=��� $��1��$��0,20,0,0q�$
��Auto=�
 � 	Resources
  �� 
TargetType �� Property �� Value �� 
TargetType ]� DecreaseRepeatButton J� Opacity ]� Thumb ]� IncreaseRepeatButton��%  '��       �� " ��.���  � ��
 ���� ��  � ��$ 8����  � �� �� " ��.R�� .+�]� $��
PART_Track����.  �� $��ScrollBar.PageUpCommand��$ 0Q� n� ���� " n�.R��� ���#BDBDBD$ 4�� �� $��ScrollBar.PageDownCommand��$ 0Q�.���� -    ��Name$ HistoryList��$��0q�
������ ���� .R��� $ 12q�$��0,0,0,1q����#E0E0E0
������ ._��� .+��� $
��Wrap=����� *	Comment+�� $ 
Horizontal=�$��0,5,0,0q�.+��� ���#757575���� *
ChangeDate+�� $�� • �����#757575$	��5,0q��� ���#2196F3���� *	ChangedBy+]� $��1��.+��� $��Комментарии��$��16(�$��Medium"�$��0,0,0,15q�$	��Top=��� $��Bottom=�$��0,15,0,0q�.+��� 
�������$��1q����#E0E0E0$ 4��._��� -    ��Name$ 
CommentBox��$��0q�$��12q�
������  �� TextWrapping$
 Wrap=� �� AcceptsReturn . $��80��" �� VerticalScrollBarVisibility$
 Auto=��� -   $-��'Добавить комментарий��$��35��$	��200��$��Right=�$��0,10,0,0q��� $
��Auto=�$��Disabled=�.���� -   $ CommentsList������ .R��� $��0,0,0,1q����#E0E0E0$ 12,15q�
������ ._��� .+��� $��Medium"����� *Author+�� $
��Wrap=�$	��0,5q����� *Text+�� ���#757575���� *Date+
# WpfToolset
The WpfToolset library is a set of components used by me when developing desktop WPF applications

Main components:
* **ObservableCollectionEx{T}** - inherits ObservableCollection and allows you to disable change notifications (for bulk changes) and provides information about added and deleted items between disabling and enabling notifications.
* **CollectionView{T}** - generic version of CollectionView
* **IndexProperty** - allows you to define indexed properties (from 1 to 3 dimensions)
* **GridEx** - inherits System.Windows.Controls.Grid, in addition it allows you to describe the rows and columns of the table using a string, for example:  
`<prefix:GridEx RowDefinitionsScript="[3] Auto;*" ColumnDefinitionScript="Auto;*">`
* **StackGrid** - inherits GridEx, automatically assigns the Grid.Row and Grid.Column properties to the nested elements in the order of their definition in the markup
* **ViewModelCommand** - implementation of System.Windows.Input.ICommand with additional features.
* **ViewModelCommandManager** - container and factory for ViewModelCommand

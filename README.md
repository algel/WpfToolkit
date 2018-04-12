# WpfTools
The Algel.WpfTools library is a set of components used by me when developing desktop WPF applications

###### It can be installed through [Nuget](https://www.nuget.org/packages/Algel.WpfTools): 
`PM> Install-Package Algel.WpfTools -Version 2018.3.30`

Main components:

* **CollectionView{T}** - generic version of CollectionView is a powerful tool to manage a collection of elements directly from the ViewModel (sorting, grouping, filtering, and the position of the current element)
* **IndexProperty** - allows you to define indexed properties (from 1 to 3 dimensions)
* **GridEx** - inherits System.Windows.Controls.Grid, in addition it allows you to describe the rows and columns of the table using a string, for example:  
`<awt:GridEx RowDefinitionsScript="[3] Auto;*" ColumnDefinitionScript="Auto;*">`
* **StackGrid** - inherits GridEx, automatically assigns the Grid.Row and Grid.Column properties to the nested elements in the order of their definition in the markup
* **ViewModelCommand** - implementation of System.Windows.Input.ICommand with additional features.
* **ViewModelCommandManager** - container and factory for ViewModelCommand

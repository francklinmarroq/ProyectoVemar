$winDir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Windows"

function Get-SharedStyles {
    return @'
    <Window.Resources>
        <Style x:Key="FieldLabel" TargetType="TextBlock">
            <Setter Property="FontSize" Value="12"/>
            <Setter Property="Foreground" Value="#64748B"/>
            <Setter Property="Margin" Value="0,0,0,4"/>
        </Style>
        <Style x:Key="FieldBox" TargetType="TextBox">
            <Setter Property="Height" Value="36"/>
            <Setter Property="Padding" Value="10,0"/>
            <Setter Property="VerticalContentAlignment" Value="Center"/>
            <Setter Property="Background" Value="White"/>
            <Setter Property="BorderBrush" Value="#E2E8F0"/>
            <Setter Property="BorderThickness" Value="1"/>
            <Setter Property="FontSize" Value="13"/>
        </Style>
        <Style x:Key="BtnPrimary" TargetType="Button">
            <Setter Property="Height" Value="36"/>
            <Setter Property="Padding" Value="20,0"/>
            <Setter Property="Background" Value="#3B82F6"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="Cursor" Value="Hand"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Border Background="{TemplateBinding Background}" CornerRadius="6" Padding="{TemplateBinding Padding}">
                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter Property="Background" Value="#2563EB"/>
                            </Trigger>
                            <Trigger Property="IsEnabled" Value="False">
                                <Setter Property="Background" Value="#93C5FD"/>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
        <Style x:Key="BtnSecondary" TargetType="Button">
            <Setter Property="Height" Value="36"/>
            <Setter Property="Padding" Value="20,0"/>
            <Setter Property="Background" Value="White"/>
            <Setter Property="Foreground" Value="#374151"/>
            <Setter Property="BorderBrush" Value="#D1D5DB"/>
            <Setter Property="BorderThickness" Value="1"/>
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="Cursor" Value="Hand"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Border Background="{TemplateBinding Background}" BorderBrush="{TemplateBinding BorderBrush}" BorderThickness="{TemplateBinding BorderThickness}" CornerRadius="6" Padding="{TemplateBinding Padding}">
                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter Property="Background" Value="#F3F4F6"/>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
    </Window.Resources>
'@
}

function Write-Window($name, $vmClass, $title, $width, $body) {
    $styles = Get-SharedStyles
    $xaml = @"
<Window x:Class="Vemar.WPF.$name"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="$title" WindowStyle="ToolWindow" ResizeMode="NoResize"
        SizeToContent="Height" Width="$width" WindowStartupLocation="CenterOwner"
        Background="#F8FAFC">
$styles
    <Border Margin="24" Background="White" CornerRadius="12" Padding="24,20">
        <Border.Effect>
            <DropShadowEffect BlurRadius="16" ShadowDepth="0" Opacity="0.06" Color="#000"/>
        </Border.Effect>
        <StackPanel>
            <StackPanel Orientation="Horizontal" Margin="0,0,0,20">
                <Border Width="4" CornerRadius="2" Background="#3B82F6" Margin="0,0,10,0"/>
                <TextBlock Text="$title" FontSize="16" FontWeight="Bold" Foreground="#0F172A" VerticalAlignment="Center"/>
            </StackPanel>
$body
            <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,12,0,0">
                <Button Name="BtnCancelar" Content="Cancelar" Style="{StaticResource BtnSecondary}" Margin="0,0,8,0"/>
                <Button Content="Guardar" Command="{Binding GuardarCommand}" Style="{StaticResource BtnPrimary}"/>
            </StackPanel>
        </StackPanel>
    </Border>
</Window>
"@
    Set-Content "$winDir\$name.xaml" -Value $xaml -Encoding UTF8

    $vb = @"
Public Class $name
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim vm = TryCast(DataContext, $vmClass)
        If vm IsNot Nothing Then
            AddHandler vm.GuardadoExitoso, Sub(s, ev) Me.Close()
        End If
    End Sub
    Private Sub BtnCancelar_Click(sender As Object, e As RoutedEventArgs) Handles BtnCancelar.Click
        Me.Close()
    End Sub
End Class
"@
    Set-Content "$winDir\$name.xaml.vb" -Value $vb -Encoding UTF8
    Write-Host "Created: $name"
}

# ---- Tipo Tramite (campo: Nombre) ----
Write-Window "AgregarTipoTramiteWindow" "TiposTramiteViewModel" "Nuevo Tipo de Trámite" "400" @'
            <TextBlock Text="Nombre" Style="{StaticResource FieldLabel}"/>
            <TextBox Text="{Binding Nombre, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
'@

# ---- Tipo Movimiento (campo: Tipo) ----
Write-Window "AgregarTipoMovimientoWindow" "TiposMovimientoViewModel" "Nuevo Tipo de Movimiento" "400" @'
            <TextBlock Text="Tipo" Style="{StaticResource FieldLabel}"/>
            <TextBox Text="{Binding Tipo, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
'@

# ---- Categoria Proyecto (campos: Nombre, CantidadLotes) ----
Write-Window "AgregarCategoriaProyectoWindow" "CategoriaProyectosViewModel" "Nueva Categoría de Proyecto" "440" @'
            <UniformGrid Columns="2" Margin="0,0,0,0">
                <StackPanel Margin="0,0,8,12">
                    <TextBlock Text="Nombre" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding Nombre, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
                <StackPanel Margin="0,0,0,12">
                    <TextBlock Text="Cantidad de Lotes" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding CantidadLotes, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
            </UniformGrid>
'@

# ---- Colaborador (campos: Nombre, Telefono, Cargo, Email) ----
Write-Window "AgregarColaboradorWindow" "ColaboradoresViewModel" "Nuevo Colaborador" "560" @'
            <UniformGrid Columns="2" Margin="0,0,0,0">
                <StackPanel Margin="0,0,8,12">
                    <TextBlock Text="Nombre" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding Nombre, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
                <StackPanel Margin="0,0,0,12">
                    <TextBlock Text="Teléfono" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding Telefono, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
                <StackPanel Margin="0,0,8,12">
                    <TextBlock Text="Cargo" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding Cargo, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
                <StackPanel Margin="0,0,0,12">
                    <TextBlock Text="Email" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding Email, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
            </UniformGrid>
'@

# ---- Contratista (campos: Nombre, Telefono) ----
Write-Window "AgregarContratistaWindow" "ContratistasViewModel" "Nuevo Contratista" "500" @'
            <UniformGrid Columns="2" Margin="0,0,0,0">
                <StackPanel Margin="0,0,8,12">
                    <TextBlock Text="Nombre" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding Nombre, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
                <StackPanel Margin="0,0,0,12">
                    <TextBlock Text="Teléfono" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding Telefono, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
            </UniformGrid>
'@

# ---- Avance (campos: Descripcion, ProyectoSeleccionado) ----
Write-Window "AgregarAvanceWindow" "AvancesViewModel" "Nuevo Avance" "500" @'
            <StackPanel Margin="0,0,0,12">
                <TextBlock Text="Proyecto" Style="{StaticResource FieldLabel}"/>
                <ComboBox ItemsSource="{Binding Proyectos}" SelectedItem="{Binding ProyectoSeleccionado}"
                          DisplayMemberPath="Nombre" Height="36" FontSize="13" Padding="8,0" VerticalContentAlignment="Center"/>
            </StackPanel>
            <StackPanel Margin="0,0,0,0">
                <TextBlock Text="Descripción" Style="{StaticResource FieldLabel}"/>
                <TextBox Text="{Binding Descripcion, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
            </StackPanel>
'@

# ---- Cobro Remedida (campos: Cantidad, RemedidaSeleccionada) ----
Write-Window "AgregarCobroRemedidaWindow" "CobroRemedidasViewModel" "Nuevo Cobro de Remedida" "500" @'
            <StackPanel Margin="0,0,0,12">
                <TextBlock Text="Remedida" Style="{StaticResource FieldLabel}"/>
                <ComboBox ItemsSource="{Binding Remedidas}" SelectedItem="{Binding RemedidaSeleccionada}"
                          DisplayMemberPath="Representante" Height="36" FontSize="13" Padding="8,0" VerticalContentAlignment="Center"/>
            </StackPanel>
            <StackPanel Margin="0,0,0,0">
                <TextBlock Text="Cantidad" Style="{StaticResource FieldLabel}"/>
                <TextBox Text="{Binding Cantidad, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
            </StackPanel>
'@

# ---- Gasto Remedida (campos: Descripcion, Monto, RemedidaSeleccionada) ----
Write-Window "AgregarGastoRemedidaWindow" "GastoRemedidasViewModel" "Nuevo Gasto de Remedida" "500" @'
            <StackPanel Margin="0,0,0,12">
                <TextBlock Text="Remedida" Style="{StaticResource FieldLabel}"/>
                <ComboBox ItemsSource="{Binding Remedidas}" SelectedItem="{Binding RemedidaSeleccionada}"
                          DisplayMemberPath="Representante" Height="36" FontSize="13" Padding="8,0" VerticalContentAlignment="Center"/>
            </StackPanel>
            <UniformGrid Columns="2" Margin="0,0,0,0">
                <StackPanel Margin="0,0,8,0">
                    <TextBlock Text="Descripción" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding Descripcion, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
                <StackPanel Margin="0,0,0,0">
                    <TextBlock Text="Monto" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding Monto, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
            </UniformGrid>
'@

# ---- Movimiento (campos: Fecha, RemedidaSeleccionada, TipoSeleccionado) ----
Write-Window "AgregarMovimientoWindow" "MovimientosViewModel" "Nuevo Movimiento" "500" @'
            <StackPanel Margin="0,0,0,12">
                <TextBlock Text="Remedida" Style="{StaticResource FieldLabel}"/>
                <ComboBox ItemsSource="{Binding Remedidas}" SelectedItem="{Binding RemedidaSeleccionada}"
                          DisplayMemberPath="Representante" Height="36" FontSize="13" Padding="8,0" VerticalContentAlignment="Center"/>
            </StackPanel>
            <UniformGrid Columns="2" Margin="0,0,0,0">
                <StackPanel Margin="0,0,8,0">
                    <TextBlock Text="Tipo" Style="{StaticResource FieldLabel}"/>
                    <ComboBox ItemsSource="{Binding Tipos}" SelectedItem="{Binding TipoSeleccionado}"
                              DisplayMemberPath="Tipo" Height="36" FontSize="13" Padding="8,0" VerticalContentAlignment="Center"/>
                </StackPanel>
                <StackPanel Margin="0,0,0,0">
                    <TextBlock Text="Fecha" Style="{StaticResource FieldLabel}"/>
                    <DatePicker SelectedDate="{Binding Fecha}" Height="36" FontSize="13"/>
                </StackPanel>
            </UniformGrid>
'@

# ---- Pago Contrato (campos: Valor, Descripcion, ContratoSeleccionado) ----
Write-Window "AgregarPagoContratoWindow" "PagoContratosViewModel" "Nuevo Pago de Contrato" "520" @'
            <StackPanel Margin="0,0,0,12">
                <TextBlock Text="Contrato" Style="{StaticResource FieldLabel}"/>
                <ComboBox ItemsSource="{Binding Contratos}" SelectedItem="{Binding ContratoSeleccionado}"
                          DisplayMemberPath="Valor" Height="36" FontSize="13" Padding="8,0" VerticalContentAlignment="Center"/>
            </StackPanel>
            <UniformGrid Columns="2" Margin="0,0,0,0">
                <StackPanel Margin="0,0,8,0">
                    <TextBlock Text="Valor" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding Valor, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
                <StackPanel Margin="0,0,0,0">
                    <TextBlock Text="Descripción" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding Descripcion, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
            </UniformGrid>
'@

# ---- Contrato (campos: Valor, ContratistaSeleccionado, ProyectoSeleccionado) ----
Write-Window "AgregarContratoWindow" "ContratosViewModel" "Nuevo Contrato" "560" @'
            <UniformGrid Columns="2" Margin="0,0,0,0">
                <StackPanel Margin="0,0,8,12">
                    <TextBlock Text="Contratista" Style="{StaticResource FieldLabel}"/>
                    <ComboBox ItemsSource="{Binding Contratistas}" SelectedItem="{Binding ContratistaSeleccionado}"
                              DisplayMemberPath="Nombre" Height="36" FontSize="13" Padding="8,0" VerticalContentAlignment="Center"/>
                </StackPanel>
                <StackPanel Margin="0,0,0,12">
                    <TextBlock Text="Proyecto" Style="{StaticResource FieldLabel}"/>
                    <ComboBox ItemsSource="{Binding Proyectos}" SelectedItem="{Binding ProyectoSeleccionado}"
                              DisplayMemberPath="Nombre" Height="36" FontSize="13" Padding="8,0" VerticalContentAlignment="Center"/>
                </StackPanel>
                <StackPanel Margin="0,0,8,0" Grid.ColumnSpan="2">
                    <TextBlock Text="Valor" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding Valor, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
            </UniformGrid>
'@

# ---- Usuario (campos: NombreUsuario, Contrasena) ----
Write-Window "AgregarUsuarioWindow" "UsuariosViewModel" "Nuevo Usuario" "440" @'
            <UniformGrid Columns="2" Margin="0,0,0,0">
                <StackPanel Margin="0,0,8,0">
                    <TextBlock Text="Nombre de Usuario" Style="{StaticResource FieldLabel}"/>
                    <TextBox Text="{Binding NombreUsuario, UpdateSourceTrigger=PropertyChanged}" Style="{StaticResource FieldBox}"/>
                </StackPanel>
                <StackPanel Margin="0,0,0,0">
                    <TextBlock Text="Contraseña" Style="{StaticResource FieldLabel}"/>
                    <PasswordBox Name="PwdBox" Height="36" Padding="10,0" VerticalContentAlignment="Center"
                                 Background="White" BorderBrush="#E2E8F0" BorderThickness="1" FontSize="13"/>
                </StackPanel>
            </UniformGrid>
'@

Write-Host "All windows created."

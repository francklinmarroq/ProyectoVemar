Public Interface IConfigurationService
    Function LoadConfig() As DatabaseConfig
    Sub SaveConfig(config As DatabaseConfig)
End Interface

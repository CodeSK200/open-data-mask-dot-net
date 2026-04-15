param location string = resourceGroup().location
param containerAppName string = 'open-data-mask-console'
param containerImage string
param keyVaultName string
param sourceConnectionStringSecretName string = 'SourceMongoConnectionString'
param destinationConnectionStringSecretName string = 'DestinationMongoConnectionString'

resource keyVault 'Microsoft.KeyVault/vaults@2024-10-01' existing = {
  name: keyVaultName
}

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2024-06-01' = {
  name: containerAppName
  location: location
  properties: {
    containers: [
      {
        name: 'app'
        properties: {
          image: containerImage
          resources: {
            requests: {
              cpu: 1.0
              memoryInGb: 1.5
            }
          }
          environmentVariables: [
            {
              name: 'SOURCE_CONNECTION_STRING'
              secureValue: keyVault.getSecret(sourceConnectionStringSecretName).value
            }
            {
              name: 'DESTINATION_CONNECTION_STRING'
              secureValue: keyVault.getSecret(destinationConnectionStringSecretName).value
            }
          ]
        }
      }
    ]
    osType: 'Linux'
    ipAddress: {
      type: 'Public'
      ports: [
        {
          protocol: 'TCP'
          port: 80
        }
      ]
    }
  }
}

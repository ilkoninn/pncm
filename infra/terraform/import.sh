#!/bin/bash
set -e

terraform import 'module.identity_service.kubernetes_deployment.this' 'pncm/identity-service'
terraform import 'module.identity_service.kubernetes_service.this' 'pncm/identity-service'
terraform import 'module.store_service.kubernetes_deployment.this' 'pncm/store-service'
terraform import 'module.store_service.kubernetes_service.this' 'pncm/store-service'
terraform import 'module.media_service.kubernetes_deployment.this' 'pncm/media-service'
terraform import 'module.media_service.kubernetes_service.this' 'pncm/media-service'
terraform import 'module.pet_service.kubernetes_deployment.this' 'pncm/pet-service'
terraform import 'module.pet_service.kubernetes_service.this' 'pncm/pet-service'
terraform import 'module.adoption_service.kubernetes_deployment.this' 'pncm/adoption-service'
terraform import 'module.adoption_service.kubernetes_service.this' 'pncm/adoption-service'
terraform import 'module.community_service.kubernetes_deployment.this' 'pncm/community-service'
terraform import 'module.community_service.kubernetes_service.this' 'pncm/community-service'
terraform import 'module.notification_service.kubernetes_deployment.this' 'pncm/notification-service'
terraform import 'module.notification_service.kubernetes_service.this' 'pncm/notification-service'
terraform import 'module.gateway.kubernetes_deployment.this' 'pncm/gateway'
terraform import 'module.gateway.kubernetes_service.this' 'pncm/gateway'

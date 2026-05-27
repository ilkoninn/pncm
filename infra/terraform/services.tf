module "identity_service" {
  source = "./modules/service"
  name   = "identity-service"
  image  = "ilkoninn/pncm-identity:latest"
}

module "store_service" {
  source = "./modules/service"
  name   = "store-service"
  image  = "ilkoninn/pncm-store:latest"
}

module "media_service" {
  source = "./modules/service"
  name   = "media-service"
  image  = "ilkoninn/pncm-media:latest"
}

module "pet_service" {
  source = "./modules/service"
  name   = "pet-service"
  image  = "ilkoninn/pncm-pet:latest"
}

module "adoption_service" {
  source = "./modules/service"
  name   = "adoption-service"
  image  = "ilkoninn/pncm-adoption:latest"
}

module "community_service" {
  source = "./modules/service"
  name   = "community-service"
  image  = "ilkoninn/pncm-community:latest"
}

module "notification_service" {
  source = "./modules/service"
  name   = "notification-service"
  image  = "ilkoninn/pncm-notification:latest"
}

module "gateway" {
  source = "./modules/service"
  name   = "gateway"
  image  = "ilkoninn/pncm-gateway:latest"
}

variable "postgres_password" {
  type    = string
  default = "postgres"
}

variable "jwt_secret" {
  type    = string
  default = "your-secret-key-here-min-32-chars!!"
}

variable "minio_access_key" {
  type    = string
  default = "minioadmin"
}

variable "minio_secret_key" {
  type    = string
  default = "minioadmin"
}

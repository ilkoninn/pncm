variable "name" {
  type = string
}

variable "image" {
  type = string
}

variable "namespace" {
  type    = string
  default = "pncm"
}

variable "env_config_map" {
  type    = string
  default = "pncm-config"
}

variable "env_secret" {
  type    = string
  default = "pncm-secrets"
}

variable "cpu_request" {
  type    = string
  default = "100m"
}

variable "memory_request" {
  type    = string
  default = "128Mi"
}

variable "cpu_limit" {
  type    = string
  default = "500m"
}

variable "memory_limit" {
  type    = string
  default = "256Mi"
}

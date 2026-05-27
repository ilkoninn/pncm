terraform {
  required_providers {
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.0"
    }
  }
}

provider "kubernetes" {
  config_path = "~/.kube/config"
}

resource "kubernetes_namespace" "pncm" {
  metadata {
    name = "pncm"
    annotations = {
      "argocd.argoproj.io/tracking-id" = "pncm:/Namespace:pncm/pncm"
    }
  }
}

resource "kubernetes_config_map" "pncm_config" {
  metadata {
    name      = "pncm-config"
    namespace = kubernetes_namespace.pncm.metadata[0].name
    annotations = {
      "argocd.argoproj.io/tracking-id" = "pncm:/ConfigMap:pncm/pncm-config"
    }
  }

  data = {
    ASPNETCORE_ENVIRONMENT = "Development"
    POSTGRES_HOST          = "pncm-postgres"
    POSTGRES_PORT          = "5432"
    POSTGRES_USER          = "postgres"
    REDIS_HOST             = "pncm-redis"
    KAFKA_BOOTSTRAP        = "pncm-kafka:29092"
    MINIO_ENDPOINT         = "pncm-minio:9000"
    MINIO_PUBLIC_ENDPOINT  = "localhost:9000"
    OTLP_ENDPOINT          = "http://pncm-jaeger:4317"
  }
}

resource "kubernetes_secret" "pncm_secrets" {
  metadata {
    name      = "pncm-secrets"
    namespace = kubernetes_namespace.pncm.metadata[0].name
    annotations = {
      "argocd.argoproj.io/tracking-id" = "pncm:/Secret:pncm/pncm-secrets"
    }
  }

  wait_for_service_account_token = false

  data = {
    POSTGRES_PASSWORD = var.postgres_password
    JWT_SECRET        = var.jwt_secret
    MINIO_ACCESS_KEY  = var.minio_access_key
    MINIO_SECRET_KEY  = var.minio_secret_key
  }
}
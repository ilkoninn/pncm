resource "kubernetes_deployment" "this" {
  metadata {
    name      = var.name
    namespace = var.namespace
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        app = var.name
      }
    }

    template {
      metadata {
        labels = {
          app = var.name
        }
      }

      spec {
        container {
          name  = var.name
          image = var.image

          port {
            container_port = 8080
          }

          env_from {
            config_map_ref {
              name = var.env_config_map
            }
          }

          env_from {
            secret_ref {
              name = var.env_secret
            }
          }

          resources {
            requests = {
              cpu    = var.cpu_request
              memory = var.memory_request
            }
            limits = {
              cpu    = var.cpu_limit
              memory = var.memory_limit
            }
          }
        }
      }
    }
  }

  lifecycle {
    ignore_changes = [
      metadata[0].annotations,
      spec[0].replicas,
      spec[0].template[0].metadata[0].annotations,
      spec[0].template[0].spec[0].container[0].env,
      spec[0].template[0].spec[0].container[0].liveness_probe,
      spec[0].template[0].spec[0].container[0].readiness_probe,
      spec[0].template[0].spec[0].automount_service_account_token,
      spec[0].template[0].spec[0].enable_service_links,
      wait_for_rollout,
    ]
  }
}

resource "kubernetes_service" "this" {
  metadata {
    name      = "pncm-${replace(var.name, "-service", "")}"
    namespace = var.namespace
  }

  spec {
    selector = {
      app = var.name
    }

    type = "ClusterIP"

    port {
      port        = 80
      target_port = 8080
    }
  }

  lifecycle {
    ignore_changes = [
      metadata[0].annotations,
      metadata[0].labels,
      wait_for_load_balancer,
    ]
  }
}

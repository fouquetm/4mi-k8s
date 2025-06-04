from locust import HttpUser, task

class HelloWorldUser(HttpUser):
    @task
    def product_list(self):
        self.client.get("/api/Product/productlist")
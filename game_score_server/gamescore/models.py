from django.db import models
from django.contrib.auth.models import User

class Score(models.Model):
    player = models.OneToOneField(User, on_delete=models.CASCADE)
    score = models.IntegerField()
    created_at = models.DateTimeField(auto_now_add=True)

    class Meta:
        ordering = ['-score']

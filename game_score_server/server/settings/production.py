from .base import *
import os

DEBUG = False
ALLOWED_HOSTS = ['192.168.1.206']  # À modifier avec votre domaine

DATABASES = {
    "default": {
        "ENGINE": "django.db.backends.sqlite3",
        "NAME": BASE_DIR / "production.sqlite3",
    }
}



REST_FRAMEWORK['DEFAULT_RENDERER_CLASSES'] = [
    'rest_framework.renderers.JSONRenderer',
]
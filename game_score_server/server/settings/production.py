from .base import *

DEBUG = False
ALLOWED_HOSTS = ['your-domain.com']  # À modifier avec votre domaine

DATABASES = {
    "default": {
        "ENGINE": "django.db.backends.sqlite3",
        "NAME": BASE_DIR / "production.sqlite3",
    }
}

# Configuration de sécurité supplémentaire
SECURE_SSL_REDIRECT = True
SESSION_COOKIE_SECURE = True
CSRF_COOKIE_SECURE = True 
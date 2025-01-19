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

# Désactivation des configurations SSL pour le développement
SECURE_SSL_REDIRECT = False
SESSION_COOKIE_SECURE = False
CSRF_COOKIE_SECURE = False

# Configuration des logs pour la production
LOGGING = {
    'version': 1,
    'disable_existing_loggers': False,
    'formatters': {
        'verbose': {
            'format': '[{asctime}] {levelname} [{name}:{lineno}] {message}',
            'style': '{',
            'datefmt': '%d/%b/%Y %H:%M:%S'
        },
    },
    'handlers': {
        'console': {
            'class': 'logging.StreamHandler',
            'formatter': 'verbose',
            'level': 'DEBUG',  # Changé à DEBUG pour voir plus de détails
        },
    },
    'loggers': {
        'django': {
            'handlers': ['console'],
            'level': 'DEBUG',  # Changé à DEBUG
            'propagate': True,
        },
        'django.server': {  # Ajout des logs du serveur
            'handlers': ['console'],
            'level': 'DEBUG',
            'propagate': False,
        },
        'gamescore': {
            'handlers': ['console'],
            'level': 'DEBUG',  # Changé à DEBUG
            'propagate': True,
        },
        'gunicorn.access': {
            'handlers': ['console'],
            'level': 'DEBUG',  # Changé à DEBUG
            'propagate': False,
        },
        'gunicorn.error': {
            'handlers': ['console'],
            'level': 'DEBUG',  # Changé à DEBUG
            'propagate': False,
        },
    },
} 
from rest_framework import serializers
from django.contrib.auth.models import User
from rest_framework.authtoken.models import Token
from .models import Score

class UserSerializer(serializers.ModelSerializer):
    password = serializers.CharField(write_only=True)
    token = serializers.CharField(read_only=True)

    class Meta:
        model = User
        fields = ['id', 'username', 'email', 'password', 'token']

    def create(self, validated_data):
        user = User.objects.create_user(
            username=validated_data['username'],
            email=validated_data['email'],
            password=validated_data['password']
        )
        # Créer un token pour l'utilisateur
        Token.objects.create(user=user)
        return user

    def to_representation(self, instance):
        data = super().to_representation(instance)
        # Ajouter le token dans la réponse
        data['token'] = Token.objects.get(user=instance).key
        return data

class ScoreSerializer(serializers.ModelSerializer):
    player = UserSerializer(read_only=True)

    class Meta:
        model = Score
        fields = ['id', 'player', 'score', 'created_at'] 
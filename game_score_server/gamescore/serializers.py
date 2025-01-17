from rest_framework import serializers
from django.contrib.auth.models import User
from .models import Score

class UserSerializer(serializers.ModelSerializer):
    class Meta:
        model = User
        fields = ('id', 'username', 'email', 'password')
        extra_kwargs = {'password': {'write_only': True}}

    def create(self, validated_data):
        user = User.objects.create_user(**validated_data)
        return user

class ScoreSerializer(serializers.ModelSerializer):
    player_name = serializers.CharField(source='player.username', read_only=True)

    class Meta:
        model = Score
        fields = ('score', 'player_name') 
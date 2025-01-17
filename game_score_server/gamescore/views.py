from django.shortcuts import render
from rest_framework import viewsets, permissions, status
from rest_framework.decorators import action
from rest_framework.response import Response
from django.contrib.auth.models import User
from .models import Score
from .serializers import ScoreSerializer, UserSerializer
from rest_framework.authtoken.models import Token

class UserViewSet(viewsets.ModelViewSet):
    queryset = User.objects.all()
    serializer_class = UserSerializer
    permission_classes = [permissions.AllowAny]
    http_method_names = ['post']  # Limiter aux créations uniquement

    def create(self, request):
        serializer = self.get_serializer(data=request.data)
        if serializer.is_valid():
            user = serializer.save()
            token, _ = Token.objects.get_or_create(user=user)
            return Response({
                'id': user.id,
                'username': user.username,
                'email': user.email,
                'token': token.key
            }, status=status.HTTP_201_CREATED)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

class ScoreViewSet(viewsets.ModelViewSet):
    queryset = Score.objects.all()
    serializer_class = ScoreSerializer
    permission_classes = [permissions.IsAuthenticated]

    def perform_create(self, serializer):
        # Récupérer le meilleur score actuel de l'utilisateur
        current_score = Score.objects.filter(player=self.request.user).first()
        new_score = serializer.validated_data['score']

        # Si pas de score existant ou si le nouveau score est meilleur
        if not current_score or new_score > current_score.score:
            Score.objects.filter(player=self.request.user).delete()
            serializer.save(player=self.request.user)

    @action(detail=False)
    def my_scores(self, request):
        scores = Score.objects.filter(player=request.user)
        serializer = self.get_serializer(scores, many=True)
        return Response(serializer.data)

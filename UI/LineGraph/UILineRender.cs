using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRender : MaskableGraphic{

        private List<float> Points;
		private int nextI = 0;
		private Texture2D texture;
		
		public float LineSize = 1;
		public int pointsLenght;

		public float scaling = 1.5f;
		public Vector2 offset = new Vector2(1, 1);
		
		void awakeLine(){
			Points = new List<float>(pointsLenght);
			texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, new Color(1, 1, 1, 1));
		}

		public void updatePoints(float newPoint){
			//Add new points to list if not full
			if (nextI < pointsLenght){
				Points[nextI] = newPoint;
				nextI++;
			}

			//Shift old points and add new point ot end of points list.
			for (int i = 0; i < pointsLenght-1; i++){
				Points[i] = Points [i+1];
			} 
			Points[pointsLenght-1] = newPoint;
		}

		void DrawGraph(){
			for (int i = 0; i < pointsLenght; i++){
				Vector2 pointCoord = getPoint(i*0.1f, Points[i]);
				Rect pointRect = new Rect(pointCoord.x, pointCoord.y, LineSize, LineSize);
				Graphics.DrawTexture(pointRect, texture);
			}
		}
		Vector2 getPoint(float x, float y){
			return new Vector2(x+offset.x, (y*scaling)/+offset.y);
		}
    }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

    // 1. Attach this to a read/write enabled sprite image
    // 2. Set the drawing_layers  to use in the raycast
    // 3. Attach a 2D collider (like a Box Collider 2D) to this sprite
    // 4. Hold down left mouse to draw on this texture!
    public class Drawable : MonoBehaviour
    {
        // PEN COLOUR
        public Color Pen_Colour = new Color(0,0,0,0);     // Change these to change the default drawing settings
        // PEN WIDTH (actually, it's a radius, in pixels)
        public int Pen_Width = 20;
        public LayerMask Drawing_Layers; //레이어 Draw로 맞춰야함
        public bool Reset_Canvas_On_Play = true;
        // The colour the canvas is reset to each time
        // public Color Reset_Colour = new Color(0, 0, 0, 255);  // By default, reset the canvas to be transparent

        // MUST HAVE READ/WRITE enabled set in the file editor of Unity
        private Sprite drawable_sprite;
        private Texture2D drawable_texture;

        private Vector2 previous_drag_position;
        private Color[] clean_colours_array;
        // Color transparent;
        private Color32[] cur_colors;
        bool mouse_was_previously_held_down = false;
        bool no_drawing_on_current_drag = false;

        // private Texture2D cloneT;
        public bool receiveSocket = false;
        public Vector2 v2; //소켓으로 받은 마우스 좌표
        public GameObject particle;
        private int sp_width;
        private SpriteRenderer spRenderer;
       
        private int count = 0;
        public GameObject coin;
        public SocketManager socketManager;
        public GameObject lastText;
        public Texture2D logoTexture;
        public GameObject bombparticle;

         Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        void Start()
        {
            
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Texture2D reimg = duplicateTexture(logoTexture);
            Rect rect = new Rect(0,0,reimg.width, reimg.height);
            Sprite sss = Sprite.Create(reimg, rect , new Vector2(0.5f, 0.5f));
            GetComponent<SpriteRenderer>().sprite = sss;
            GetComponent<BoxCollider2D>().size = new Vector2(reimg.width/100, reimg.height/100);
            spRenderer = GetComponent<SpriteRenderer>();
            drawable_sprite = this.GetComponent<SpriteRenderer>().sprite;
           
            // Sprite s = Resources.Load<Sprite>("Logo/gsmatt2"); //원래 이미지
            
            sp_width = (int)drawable_sprite.rect.width; //(int)drawable_sprite.rect.width;
            
            drawable_texture = drawable_sprite.texture;
            
         
            

            // Initialize clean pixels to use
            clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
            clean_colours_array = drawable_texture.GetPixels();// drawable_texture.GetPixels();
            // for (int x = 0; x < clean_colours_array.Length; x++)
            //     clean_colours_array[x] = Reset_Colour;
    

            // Should we reset our canvas image when we hit play in the editor?
            if (Reset_Canvas_On_Play) {
                ResetCanvas();
            } 

      
        }


        private bool check = true;
        void Update()
        {
           
            if (count >= 50008000) {
                socketManager.startFadeOut();
                count = 0;
                check = false;
                // Debug.Log("color half = " + count);
                socketManager.fading = true;
                bombparticle.SetActive(true);
                StartCoroutine( fadeOut() );
            }

            // Is the user holding down the left mouse button?
            // bool mouse_held_down = Input.GetMouseButton(0);
            if (receiveSocket) //if (mouse_held_down && !no_drawing_on_current_drag)
            {
                if (check) {
                    Color[] cc = drawable_texture.GetPixels();
                    // Debug.Log("cc length = " + cc.Length );
                    if(cc != null) {
                        for(int i=0; i<cc.Length; i++) {
                        
                            if (cc[i].a == 0) {
                                // Debug.Log("0000000");
                                count++;
                            }
                        }
                    }
                }
                   
                lastText.SetActive(false);
                // textAnimation.SetActive(false);

                if (showParticle) {

                    particle.SetActive(true);
                    particle.transform.position = new Vector3(v2.x, v2.y, -0.7f);
                    coin.SetActive(true);
                    coin.transform.position = new Vector3(v2.x, v2.y, -0.8f);
                }
                
                
                // Convert mouse coordinates to world coordinates
                // Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // socketManager.sendDrawing( mouse_world_position );
                // Check if the current mouse position overlaps our image
                Collider2D hit = Physics2D.OverlapPoint(v2, Drawing_Layers.value);

                if (hit != null && hit.transform != null) {
                    // We're over the texture we're drawing on! Change them pixel colours
                    ChangeColourAtPoint(v2);
                    
                }else{
                    
                    // We're not over our destination texture
                    previous_drag_position = Vector2.zero;
                    if (!mouse_was_previously_held_down)
                    {
                        // This is a new drag where the user is left clicking off the canvas
                        // Ensure no drawing happens until a new drag is started
                        no_drawing_on_current_drag = true;
                    }
                }
            }
            // Mouse is released
            // else if (!mouse_held_down)
            // {
            //     previous_drag_position = Vector2.zero;
            //     no_drawing_on_current_drag = false;
               
            // }
            // // else {
            // //    particle.SetActive(false);
            // //  coin.SetActive(false);  
            // // }
            // mouse_was_previously_held_down = mouse_held_down;
        }

    

        
        private Vector2 v2BeforeDraw;
        // Pass in a point in WORLD coordinates
        // Changes the surrounding pixels of the world_point to the static pen_colour
        public void ChangeColourAtPoint(Vector2 world_point)
        {
            // Change coordinates to local coordinates of this image
            Vector3 local_pos = transform.InverseTransformPoint(world_point);
           
            // Change these to coordinates of pixels
            float pixelWidth = drawable_sprite.rect.width;
            float pixelHeight = drawable_sprite.rect.height;
            float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x; // 100
           
            // Need to center our coordinates
            float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
            float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

            // Round current mouse position to nearest pixel
            Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

            cur_colors = drawable_texture.GetPixels32();

            if (previous_drag_position == Vector2.zero)
            {
                // If this is the first time we've ever dragged on this image, simply colour the pixels at our mouse position
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                
                // Colour in a line from where we were on the last update call
                 if(Vector2.Distance(v2BeforeDraw, previous_drag_position) > 10){
                   
                    v2BeforeDraw = previous_drag_position;
                    ColourBetween(previous_drag_position, pixel_pos);

                 }
            }
            ApplyMarkedPixelChanges();

            //Debug.Log("Dimensions: " + pixelWidth + "," + pixelHeight + ". Units to pixels: " + unitsToPixels + ". Pixel pos: " + pixel_pos);
            previous_drag_position = pixel_pos;
        }


        // Set the colour of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is coloured
        public void ColourBetween(Vector2 start_point, Vector2 end_point)
        {
            // Get the distance from start to finish
            float distance = Vector2.Distance(start_point, end_point);
            Vector2 direction = (start_point - end_point).normalized;

            Vector2 cur_position = start_point;

            // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
            float lerp_steps = 1 / distance;
           
            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                cur_position = Vector2.Lerp(start_point, end_point, lerp);
                MarkPixelsToColour(cur_position, Pen_Width, Pen_Colour);
            }
        }





        public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            int extra_radius = Mathf.Min(0, pen_thickness - 2);

            Vector2 center_p = new Vector2(center_x, center_y);
            int left_x = center_x - pen_thickness;
            int right_x = center_x + pen_thickness;
            int left_y = center_y - pen_thickness;
            int right_y = center_y + pen_thickness;

            for (int x = left_x; x <= right_x; x += 4)
            {
                // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
                if (x >= sp_width || x < 0) {

                    continue;  
                }     
                    
                for (int y = left_y; y <= right_y; y += 22)
                {
                    
                    if (x >= sp_width || x < 0) {
                        
                        continue;  
                    }     
                  
                    if(Vector2.Distance(new Vector2(x, y), center_p ) < pen_thickness){

                         MarkPixelToChange(x, y, color_of_pen);
                    }

                }
            }

        }

        public void MarkPixelToChange(int x, int y, Color color)
        {
            
            // Need to transform x and y coordinates to flat coordinates of array
            int array_pos = y * (int)drawable_sprite.rect.width + x;

          
            // Check if this is a valid position
            if (array_pos > (cur_colors.Length -1) || array_pos < 0)
                return;

            if (cur_colors[array_pos] == color) {
                // Debug.Log(" already color ");
                return;
            }

            cur_colors[array_pos] = color;
        }


        public void ApplyMarkedPixelChanges()
        {
            drawable_texture.SetPixels32(cur_colors);
            drawable_texture.Apply();
            receiveSocket = false; //픽셀 바꾸고 업데이트문 정지하려고
            
        }


        // Directly colours pixels. This method is slower than using MarkPixelsToColour then using ApplyMarkedPixelChanges
        // SetPixels32 is far faster than SetPixel
        // Colours both the center pixel, and a number of pixels around the center pixel based on pen_thickness (pen radius)
        public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    drawable_texture.SetPixel(x, y, color_of_pen);
                }
            }

            drawable_texture.Apply();
        }


        // Changes every pixel to be the reset colour

        public void ResetCanvas()
        {
        
            count = 0;
            check = true;
            showParticle = true;

            spRenderer.color = new Color(255,255,255,255);
            drawable_texture.SetPixels(clean_colours_array);
            drawable_texture.Apply();

        
        }

        public void clickButton() {

            // Texture2D sp =  this.GetComponent<SpriteRenderer>().sprite.texture;
            // sp = cloneT;
            // sp.SetPixels32( cloneT.GetPixels32() );
            // sp.Apply();
            
            // drawable_texture.SetPixels32( cloneT.GetPixels32() );
            // drawable_texture.Apply();

            ResetCanvas();
        }

       
        private float animTime = 2.0f;
	    private float time = 0f;
        private bool showParticle = true; // 다 긁은후에 파티클,코인 안보이게
        IEnumerator fadeOut() {
            
            Color color = spRenderer.color;
            time = 0f;
            color.a = Mathf.Lerp(1f, 0f, time);
            
            while ( color.a > 0f ) {
                
                time += Time.deltaTime / animTime;
                color.a = Mathf.Lerp(1f, 0f, time);
                spRenderer.color = color;

                yield return null;
            }
            
            socketManager.fading = false;
            socketManager.endFadeOut();
            coin.SetActive(false);
            particle.SetActive(false);
            showParticle = false;
            yield return new WaitForSeconds(2.9f);
            bombparticle.SetActive(false);

	    }

      
    }

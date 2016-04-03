package no.evilevilevil.evildroid;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.AsyncTask;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnTouchListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.UnsupportedEncodingException;

public class MainActivity extends AppCompatActivity implements OnTouchListener {
    TextView _view;
    Button _buttonScan;
    Button _buttonSend;
    FrameLayout _frameLayout;
    ImageView _imageView;
    WifiManager wifi;
    EditText _editText;

    String _macaddress;
    int cal_x = 0;
    int cal_y = 0;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        _frameLayout = (FrameLayout)findViewById(R.id.framelayout);
        _frameLayout.setOnTouchListener(this);
        _imageView = (ImageView)findViewById(R.id.imageView);
        _editText = (EditText)findViewById(R.id.editText);
        _buttonScan = (Button)findViewById(R.id.scanButton);
        _buttonSend = (Button)findViewById(R.id.sendButton);

        wifi = (WifiManager) getSystemService(Context.WIFI_SERVICE);
        WifiInfo info = wifi.getConnectionInfo();
        _macaddress = info.getMacAddress();
        _editText.setText(_macaddress);

        _buttonScan.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                Log.d("GOGO", "Scanning");
                // Perform action on click
                wifi.startScan();
                Toast.makeText(getApplicationContext(), "Scanning wifi", Toast.LENGTH_SHORT).show();
            }
        });
        _buttonSend.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                Log.d("GOGO", "oooooooooooh... send some calibration data");
                //postCalibration(_editTfjsext.getText().toString(), cal_x, cal_y);
                String url = "http://relay-dev.westeurope.cloudapp.azure.com:8888/calibrate";
                //makeRequest(url, "{ \"hwaddr\": \"123\", 'x': 10, 'y': 10}");
                _macaddress = _editText.getText().toString();
                Log.d("MAC", _macaddress);
                new HttpAsyncTask().execute("");
            }
        });
        registerReceiver(new BroadcastReceiver() {
            @Override
            public void onReceive(Context c, Intent intent) {
                //results = wifi.getScanResults();
                //size = results.size();
                Toast.makeText(getApplicationContext(), "Scanning wifi done", Toast.LENGTH_SHORT).show();
            }
        }, new IntentFilter(WifiManager.SCAN_RESULTS_AVAILABLE_ACTION));


    }
    @Override
    public boolean onTouch(View view, MotionEvent event) {
        final int X = (int)event.getX();
        final int Y = (int)event.getY();
        cal_x = (int)(100 * X ) / _frameLayout.getWidth();
        cal_y = (int)(100 * Y ) / _frameLayout.getHeight();
        // clamp them
        if (cal_x > 100)
                cal_x = 100;
        if (cal_y > 100)
                cal_y = 100;
        if (cal_y < 0)
                cal_y = 0;
        if (cal_x < 0)
                cal_x = 0;


        _imageView.setX(X);
        _imageView.setY(Y);
        Log.d("ATAG", String.format("WHAAAAAAAAAAAAT raw: %d x %d", X, Y));
        Log.d("ATAG", String.format("WHAAAAAAAAAAAAT calc: %d x %d", cal_x, cal_y));
        Log.d("ATAG", String.format("WHAAAAAAAAAAAAT lay: %d x %d", _frameLayout.getWidth(), _frameLayout.getHeight()));
        return true;
    }

    public static HttpResponse makeRequest(String uri, String json) {
        try {
            HttpPost httpPost = new HttpPost(uri);
            httpPost.setEntity(new StringEntity(json));
            httpPost.setHeader("Accept", "application/json");
            httpPost.setHeader("Content-type", "application/json");
            return new DefaultHttpClient().execute(httpPost);
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        } catch (ClientProtocolException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
        return null;
    }
    public static String postCalibration(String hwaddr, int x, int y){
        String url = "http://relay-dev.westeurope.cloudapp.azure.com:8888/calibrate";
        InputStream inputStream = null;
        String result = "";
        try {

            // 1. create HttpClient
            HttpClient httpclient = new DefaultHttpClient();

            // 2. make POST request to the given URL
            HttpPost httpPost = new HttpPost(url);

            String json = "";

            // 3. build jsonObject
            JSONObject jsonObject = new JSONObject();
            Log.d("PCS", "aa1");
            jsonObject.accumulate("hwaddr", hwaddr);
            jsonObject.accumulate("x", new Integer(x));
            jsonObject.accumulate("y", new Integer(y));

            // 4. convert JSONObject to JSON to String
            json = jsonObject.toString();
            Log.d("JSON", json);

            // 5. set json to StringEntity
            StringEntity se = new StringEntity(json);

            // 6. set httpPost Entity
            httpPost.setEntity(se);

            // 7. Set some headers to inform server about the type of the content
            httpPost.setHeader("Accept", "application/json");
            httpPost.setHeader("Content-type", "application/json");

            // 8. Execute POST request to the given URL
            HttpResponse httpResponse = httpclient.execute(httpPost);

            // 9. receive response as inputStream
            inputStream = httpResponse.getEntity().getContent();

            // 10. convert inputstream to string
            if(inputStream != null)
                result = convertInputStreamToString(inputStream);
            else
                result = "Did not work!";

        } catch (Exception e) {
            Log.d("InputStream", e.getLocalizedMessage());
        }

        // 11. return result
        Log.d("AAAA", "DONE");
        return result;
    }

    private static String convertInputStreamToString(InputStream inputStream) throws IOException {
        BufferedReader bufferedReader = new BufferedReader( new InputStreamReader(inputStream));
        String line = "";
        String result = "";
        while((line = bufferedReader.readLine()) != null)
            result += line;

        inputStream.close();
        return result;

    }
    private class HttpAsyncTask extends AsyncTask<String, Void, String> {
        @Override
        protected String doInBackground(String... urls) {
            String url = "http://relay-dev.westeurope.cloudapp.azure.com:8888/calibrate";
            //makeRequest(url, "{ \"hwaddr\": \"123\", 'x': 10, 'y': 10}");
            postCalibration(_macaddress, cal_x, cal_y);
       return "";
        }
        // onPostExecute displays the results of the AsyncTask.
        @Override
        protected void onPostExecute(String result) {
            Toast.makeText(getBaseContext(), "Data Sent!", Toast.LENGTH_LONG).show();
        }
    }
}

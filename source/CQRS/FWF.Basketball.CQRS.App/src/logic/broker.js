import axios from 'axios'

export default function() {
      
    // Set config defaults when creating the broker
    const instance = axios.create({
      baseURL:  process.env.BROKER_URI
    });

    // Wrap the axios instance with methods to simplify message communication
    var broker = {

      get : async function(name, payload) {
        var post = [
          {
            "name": name,
            "version": "1.0.0.0",
            "payload": payload 
          }
        ]
    
        var response = null 

        try {
          response = await instance.post('/', post)
        } catch (ex) {
          console.error(ex);
        }

        if (response && response.data) {
          return response.data[0].payload;
        }

        return null 
      }
    }

    return broker;
};

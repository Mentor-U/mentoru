<h1> MentorU </h1>

<par>
Build and run in Visual Studio 2019 (16.9.3) on windows or Visual Studio for Mac (8.9.3). Emulator Versions: iOS 14.4, Android 29.

<h2> Abstract </h2>
<par>
    Do you ever wonder if the classes you are taking will set you up for success in your desired career? MentorU is a cross platform mobile application to connect alumni and students from the University of Utah to facilitate a social and interactive mentoring platform for students seeking guidance. We hope this encourages students to make better career choices and stay motivated to finish their degrees by matching them with alumni in the industry that best interests them.
</par>
<br/>
<br/>
<par>
    Not every student has a system in place to be supported through school. Some lack resources and safe spaces to work, and many do not have role models to encourage their learning. MentorU is a platform where students can find this support from their local community. By engaging Alumni, especially recently graduated students, we hope to create a support system where books, equipment, knowledge, and career advice can be given to students as they begin to acclimate to college life. We can also provide resources for setting up meetings and mentoring in person in safe places, such as coffee shops, and a rating/review feature to find the best spots for both you and your mentor. Another key feature is the marketplace, where students and grads can hand down or sell old textbooks and resources to students in need, without becoming a mentor or signing up.
</par>

<h2> Background </h2>
<par>
    MentorU aims to foster a community at the University of Utah and bridge the gap between school and industry. Existing mentoring platforms have been around for some time now and do well at supplying career advice to broad areas of interest. There are other applications that provide mentor-like services, such as; LinkedIn, Chegg, Piazza, and Qooper. Marketplace services are typically available through the Facebook Marketplace or KSL Classifieds in the midwest.
</par>
<br/>
<br/>
<par>
    LinkedIn helps students connect to potential career opportunities, but this does not let them establish a relationship that is personal and can get them homework help. LinkedIn is very broad and the people on it are typically industry professionals sharing experiences and knowledge in their realm of work. Platforms like Chegg allow you to get general homework help, but lack in identifying the credibility of the source or what kind of mentor you would be paired with. It has no long term mission, and will only provide you with situational tutoring. Discussion boards like Piazza allow you to ask general questions, but lack one on one support and the networking features are difficult to use and navigate, as the UI is very clunky and it isn’t widely used or known about. Qooper is a closed platform that allows for mentoring programs to be created with features similar to Mentor U. However, it's not a free application, and it's difficult to get information on pricing and confusing to set up. It is more of a white label software that an organization could pay to have implemented for them. Facebook Marketplace and KSL Classifieds are full of random listings, and a chore to find the specific textbook you need, if any are even listed.
</par>
<br/>
<br/>
<par>
    Mentor U will be specifically tailored to University of Utah students, and will be free for them and the school. It will provide more specific connections for students, since the mentors will be Utah Alumni who understand the program and students' situation best. This is especially important given that the University of Utah is a commuter school, meaning that even when school is in person, people can struggle to make connections in school. Additionally, Mentor U creates a sustainable and economically friendly path for students to acquire materials by taking advantage of students finishing courses, allowing them to sell their used items to the next generation of students in the class. Future students won’t be paying retail prices for materials, and the old students will be encouraged to get a return on their investments by putting their no longer needed materials for sale. Mentor U is able to do all of these things, while the competition falls short.
</par>

<h2> Analysis </h2>
<par>
    Our application will follow a Model View ViewModel (MVVM) software architecture. The ViewModels will act like a controller and will contain all the logic for interacting with the application as well as handling all requests made to the database. The View will send out API requests to the user connections and marketplace API’s, which will handle the requests and update the Views as needed. For example, if a user wants to search for a mentor in the mentor searching view, any keyword/name they search up will be sent as an API post request to its intended ViewModel which in turn will query the database. The query will return data with matching mentors to the viewModel which will then update the mentor searching view with the matching mentors. The chat feature will follow a peer to peer system between users and mentors. Chatting with a bot will follow a similar type of architecture, but it will have the ability to send requests to the chat API which will be handled by its ViewModel.
</par>
<br/>
<br/>
<par>
    The application database will contain any information about the users, marketplace items, and chat history. ViewModels will access the database using a database service specific to Xamarin to query any needed data. The Azure Bot Service will provide all the tools for natural language processing and will support the AssistantU feature. When requests are sent to the chat API, and the recipient is the AssistantU bot, the chats ViewModel will process the responses and query the database for relevant mentors. As requests are sent to the ViewModels, they will have access to the notifications module and as they are handling the request they will be able to generate notifications that are needed to reflect any updates. Additionally, the Azure Active Directory (AAD) will be used to give our application some form of identity management, which will handle user/account authentication and authorization.
</par>

<h2> Required Libraries: </h2>

    Azure.Storage.Blobs (12.8.0)
    Microsoft.AspNetCore.SignalR (1.1.0)
    Mircrostoft.AspNetCore.SignalR.Client (5.0.4)
    Microsoft.Azure.Mobile.Client (4.2.0)
    Microsoft.Bot.Connector.DirectLine (3.0.2)
    Microsoft.Extensions.Caching.Memory (5.0.0)
    Microsoft.Identity.Client (4.27.0)
    Microsoft.Rest.ClientRuntime (2.3.22)
    Newtonsoft.Json (12.0.3)
    Plugin.Permissions(6.0.1)
    Rg.Plugins.Popup (2.0.0.10)
    sqlite-net-pcl (1.7.335)
    System.ComponentModel.Annotations (5.0.0)
    System.Rflection.Emit (4.7.0)
    Xam.Plugin.Media (5.0.1)
    Xam.Plugins.Forms.ImageCirlce (3.0.0.5)
    Xamarin.Essentials (1.6.1)
    Xamarin.FFImageLoading.Forms (2.4.11.982)
    Xamarin.FFImageLoading.Transformations (2.4.11.982)
    Xamarin.Forms (5.0.0.2012)
    Xamarin.Plugin.FilePicker (2.1.41)


</par>

import React, { useState, useRef, useEffect } from 'react';
import { Video, Circle, Square } from 'lucide-react';

const VideoUploadTest = () => {
    const [recording, setRecording] = useState(false);
    const [stream, setStream] = useState(null);
    const [uploadStatus, setUploadStatus] = useState(null);
    const [totalSize, setTotalSize] = useState(0);
    const [uploadedChunks, setUploadedChunks] = useState([]);

    const videoRef = useRef(null);
    const mediaRecorderRef = useRef(null);
    const chunksRef = useRef([]);
    const startTimeRef = useRef(null);

    const startRecording = async () => {
        try {
            const mediaStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
            videoRef.current.srcObject = mediaStream;
            setStream(mediaStream);

            // Initialize media recorder
            const mediaRecorder = new MediaRecorder(mediaStream);
            mediaRecorderRef.current = mediaRecorder;

            mediaRecorder.ondataavailable = async (e) => {
                if (e.data.size > 0) {
                    chunksRef.current.push(e.data);
                    await uploadChunk(e.data);
                }
            };

            // Start recording in 1-second chunks
            mediaRecorder.start(1000);
            setRecording(true);
            startTimeRef.current = Date.now();
            setUploadStatus('Recording and uploading...');
        } catch (err) {
            console.error('Error starting recording:', err);
            setUploadStatus('Error: ' + err.message);
        }
    };

    const stopRecording = () => {
        if (mediaRecorderRef.current && recording) {
            mediaRecorderRef.current.stop();
            stream.getTracks().forEach(track => track.stop());
            setRecording(false);
            setUploadStatus('Upload complete');
        }
    };

    const uploadChunk = async (chunk) => {
        try {
            // Create form data
            const formData = new FormData();
            formData.append('file', chunk);
            formData.append('uploadUrl', 'YOUR_BUNNY_UPLOAD_URL'); // Replace with actual URL
            formData.append('startByte', totalSize);

            const response = await fetch('/api/VideoUpload/chunk', {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                setTotalSize(prev => prev + chunk.size);
                setUploadedChunks(prev => [...prev, {
                    startByte: totalSize,
                    size: chunk.size,
                    timestamp: Date.now() - startTimeRef.current
                }]);
            } else {
                console.error('Upload failed:', await response.text());
                setUploadStatus('Upload failed');
            }
        } catch (err) {
            console.error('Error uploading chunk:', err);
            setUploadStatus('Upload error: ' + err.message);
        }
    };

    // Cleanup on unmount
    useEffect(() => {
        return () => {
            if (stream) {
                stream.getTracks().forEach(track => track.stop());
            }
        };
    }, [stream]);

    return (
        <div className="max-w-2xl mx-auto p-4 space-y-4">
            <div className="bg-gray-100 p-4 rounded-lg">
                <h2 className="text-lg font-semibold mb-4">Video Upload Test</h2>

                {/* Video Preview */}
                <div className="relative aspect-video bg-black rounded-lg overflow-hidden mb-4">
                    <video
                        ref={videoRef}
                        autoPlay
                        muted
                        playsInline
                        className="w-full h-full object-cover"
                    />

                    {/* Recording indicator */}
                    {recording && (
                        <div className="absolute top-4 right-4 flex items-center space-x-2 bg-black/50 text-white px-3 py-1 rounded-full">
                            <Circle className="w-4 h-4 text-red-500 animate-pulse" />
                            <span className="text-sm">Recording</span>
                        </div>
                    )}
                </div>

                {/* Controls */}
                <div className="flex justify-center space-x-4">
                    {!recording ? (
                        <button
                            onClick={startRecording}
                            className="flex items-center space-x-2 bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
                        >
                            <Video className="w-5 h-5" />
                            <span>Start Recording</span>
                        </button>
                    ) : (
                        <button
                            onClick={stopRecording}
                            className="flex items-center space-x-2 bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600"
                        >
                            <Square className="w-5 h-5" />
                            <span>Stop Recording</span>
                        </button>
                    )}
                </div>
            </div>

            {/* Upload Status */}
            <div className="bg-gray-100 p-4 rounded-lg">
                <h3 className="font-semibold mb-2">Upload Status</h3>
                <p className="text-sm text-gray-600 mb-2">{uploadStatus}</p>

                {/* Upload Progress */}
                <div className="space-y-2">
                    <div className="text-sm text-gray-600">
                        Total Size: {(totalSize / 1024).toFixed(2)} KB
                    </div>
                    <div className="text-sm text-gray-600">
                        Chunks Uploaded: {uploadedChunks.length}
                    </div>

                    {/* Chunk Timeline */}
                    {uploadedChunks.length > 0 && (
                        <div className="mt-4">
                            <h4 className="text-sm font-semibold mb-2">Upload Timeline</h4>
                            <div className="h-24 relative border border-gray-200 rounded">
                                {uploadedChunks.map((chunk, i) => (
                                    <div
                                        key={i}
                                        className="absolute bottom-0 bg-blue-400 w-2"
                                        style={{
                                            height: `${(chunk.size / (1024 * 10)) * 100}%`,
                                            left: `${(chunk.timestamp / (30 * 1000)) * 100}%`
                                        }}
                                        title={`Size: ${(chunk.size / 1024).toFixed(2)}KB\nTime: ${(chunk.timestamp / 1000).toFixed(1)}s`}
                                    />
                                ))}
                            </div>
                            <div className="text-xs text-gray-500 mt-1">
                                Timeline: 30 seconds, Height represents chunk size
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default VideoUploadTest;